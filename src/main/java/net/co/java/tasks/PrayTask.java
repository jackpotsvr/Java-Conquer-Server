package net.co.java.tasks;

import net.co.java.entity.Entity;
import net.co.java.entity.Player;
import net.co.java.packets.UpdatePacket;

/**
 * Created by Thomas on 04/04/2015.
 */
public class PrayTask extends EntityTickTask<Player>  {
    protected Player caster;

    protected PrayTask(Player entity, Player caster) {
        super(entity);
        this.caster = caster;
    }

    @Override
    public boolean isInterruptedByInteraction() {
        return true;
    }

    @Override
    public Runnable getRunnable() {
        return () -> {
            entity.setPrayerHost(caster);
            entity.setFlag(Entity.Flag.PRAY);
            new UpdatePacket(entity)
                    .setAttribute(UpdatePacket.Mode.RaiseFlag, entity.getFlags())
                    .build().send(entity);

            entity.setBlessTime(entity.getBlessTime() + 1);
            sendUpdateBlessingPacket();

        };
    }


    @Override
    public Runnable getInterruptedRunnable() {
        return () -> {
            super.getInterruptedRunnable().run();
            entity.setPrayerHost(null);
            entity.removeFlag(Entity.Flag.PRAY);

            new UpdatePacket(entity)
                    .setAttribute(UpdatePacket.Mode.RaiseFlag, entity.getFlags())
                    .build().send(entity);

            gameServerTicks.addTickTask(new RemoveLuckyTime(entity));


//            Player p = bless.getCaster();
//
//            p.removeFlag(Entity.Flag.LUCKYTIME);
//            p.setPrayerHost(null);
//
//            new UpdatePacket(p)
//                    .setAttribute(UpdatePacket.Mode.RaiseFlag, p.getFlags())
//                    .build().send(p);
//
//            // Start removing the LuckyTime...
//            gameServerTicks.addTickTask(new RemoveLuckyTime(entity));
        };
    }

    @Override
    public long getPeriod() {
        return 1000;
    }

    @Override
    public long getInitialDelay() {
        return 2000;
    }


    protected void sendUpdateBlessingPacket() {
        new UpdatePacket(entity)
                .setAttribute(UpdatePacket.Mode.LuckyTime, entity.getBlessTime() * 1000)
                .build().send(entity);
    }
}
