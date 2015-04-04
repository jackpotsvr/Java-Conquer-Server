package net.co.java.tasks;

import net.co.java.entity.Player;
import net.co.java.packets.UpdatePacket;

/**
 * Created by Thomas on 03/04/2015.
 */
public class RemoveLuckyTime extends EntityTickTask<Player> {

    protected RemoveLuckyTime(Player entity) {
        super(entity);
    }

    @Override
    public boolean isInterruptedByInteraction() {
        return false;
    }

    @Override
    public Runnable getRunnable() {
        return () ->  {
            entity.setBlessTime(entity.getBlessTime() - 1);
            sendUpdateBlessingPacket();
        };
    }

    @Override
    public long getPeriod() {
        return 1000;
    }

    protected void sendUpdateBlessingPacket() {
        new UpdatePacket(entity)
                .setAttribute(UpdatePacket.Mode.LuckyTime, entity.getBlessTime() * 1000)
                .build().send(entity);
    }
}
