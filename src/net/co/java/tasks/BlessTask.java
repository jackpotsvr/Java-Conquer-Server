package net.co.java.tasks;

import net.co.java.entity.Entity;
import net.co.java.entity.Player;
import net.co.java.packets.UpdatePacket;
import net.co.java.skill.Bless;

import java.util.concurrent.TimeUnit;

/**
 * Created by Thomas on 03/04/2015.
 */
public class BlessTask extends EntityTickTask<Player> {
    private final Bless bless;
    private int tickCount = 0;

    public BlessTask(Bless bless, Player entity) {
        super(entity);
        this.bless = bless;
    }

    @Override
    public Runnable getRunnable() {
        return () -> {
            if(bless.getCasterId() == entity.getIdentity()) {
                entity.setBlessTime(entity.getBlessTime() + 1);
                sendUpdateBlessingPacket();
            }
            else
                if(tickCount++%3 != 0) {
                    entity.setBlessTime(entity.getBlessTime() + 1);
                    sendUpdateBlessingPacket();
                }
        };
    }

    protected void sendUpdateBlessingPacket() {
        new UpdatePacket(entity)
                .setAttribute(UpdatePacket.Mode.LuckyTime, entity.getBlessTime() * 1000)
                .build().send(entity);
    }

    @Override
    public Runnable getInterruptedRunnable() {
        return () -> {
            super.getInterruptedRunnable().run();
            Player p = bless.getCaster();

            p.removeFlag(Entity.Flag.LUCKYTIME);
            p.setPrayerHost(null);

            new UpdatePacket(p)
                    .setAttribute(UpdatePacket.Mode.RaiseFlag, p.getFlags())
                    .build().send(p);

            // Start removing the LuckyTime...
            gameServerTicks.addTickTask(new RemoveLuckyTime(entity));
        };
    }

    @Override
    public long getPeriod() {
        return 333333333;  /* 1/3 of a second, in nano seconds */
    }

    @Override
    public TimeUnit getTimeUnit() {
        return TimeUnit.NANOSECONDS;
    }

    @Override
    public boolean isInterruptedByInteraction() {
        return true;
    }

    @Override
    public long getInitialDelay() {
        return 1000000000; // 1 second, to avoid people getting an advantage by spamming Bless skill.
    }
}
