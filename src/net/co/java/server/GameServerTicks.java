package net.co.java.server;

import net.co.java.entity.Entity;
import net.co.java.tasks.EntityTickTask;
import net.co.java.tasks.TickTask;

import java.util.ArrayList;
import java.util.Collections;
import java.util.HashMap;
import java.util.List;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.ScheduledFuture;


public class GameServerTicks {
    protected final ScheduledExecutorService scheduler =
            Executors.newScheduledThreadPool(8);

    protected final java.util.Map<Long, List<EntityTickTask>> entityExecutors =
            Collections.synchronizedMap(new HashMap<>());

    public List<EntityTickTask> getTasksForEntity(long entityId) {
        return entityExecutors.get(entityId);
    }

    public void addTickTask(TickTask task){
        final ScheduledFuture<?> handle =
                scheduler.scheduleAtFixedRate(task.getRunnable(), task.getInitialDelay(),
                        task.getPeriod(), task.getTimeUnit());
        if(task instanceof EntityTickTask) {
            EntityTickTask entityTickTask = (EntityTickTask) task;
            entityTickTask.setScheduledFuture(handle);
            entityTickTask.setGameServerTicks(this);
            getTasksForEntity(entityTickTask.getIdentity()).add(entityTickTask);
        }
    }

    public synchronized void addEntity(Entity e) {
        if(!entityExecutors.containsKey(e.getIdentity())) {
            List<EntityTickTask> list = new ArrayList<>();
            entityExecutors.put(e.getIdentity(), list);
        }
    }

    public synchronized void removeEntity(Entity e) {
        if(entityExecutors.containsKey(e.getIdentity())) {
            entityExecutors.remove(e.getIdentity());
        }
    }

    public synchronized void didInteract(Entity e) {
        getTasksForEntity(e.getIdentity()).stream().filter(ett -> ett.isInterruptedByInteraction()).forEach(ett -> {
            ett.getScheduledFuture().cancel(true);
            scheduler.submit(ett.getInterruptedRunnable());
        });
    }
}
