package net.co.java.tasks;

import net.co.java.entity.Entity;
import net.co.java.server.GameServerTicks;

import java.util.concurrent.ScheduledFuture;

/**
 * Created by Thomas on 03/04/2015.
 */
public abstract class EntityTickTask<E extends Entity> extends TickTask {
    protected ScheduledFuture<?> scheduledFuture;

    protected GameServerTicks gameServerTicks;

    protected E entity;

    protected EntityTickTask(E entity) {
        this.entity = entity;
    }

    public long getIdentity() {
        return entity.getIdentity();
    }

    /**
     * @return the Runnable that should ran on interruption.
     * if you don't want anything to be run on interruption then simply
     * note that you should always invoke super.getInterruptedRunnable() when overriding this.
     * return () -> {};
     */
    public Runnable getInterruptedRunnable() {
        return () ->  {
            scheduledFuture.cancel(true);
        };
    }

    /**
     * @return returns whether the Task should be stopped if the entity performs
     * any interaction such as movement.
     */
    public abstract boolean isInterruptedByInteraction();

    public ScheduledFuture<?> getScheduledFuture() {
        return scheduledFuture;
    }

    public void setScheduledFuture(ScheduledFuture<?> scheduledFuture) {
        this.scheduledFuture = scheduledFuture;
    }
    public void setGameServerTicks(GameServerTicks gameServerTicks) {
        this.gameServerTicks = gameServerTicks;
    }
}
