package net.co.java.server;

import net.co.java.packets.Packet;
import net.co.java.packets.PacketType;
import net.co.java.packets.serialization.Type;
import org.reflections.Reflections;
import java.util.AbstractMap;

import java.util.ArrayList;
import java.util.List;

public class Packets {
    private static Packets instance;

    private List<AbstractMap.SimpleEntry<PacketType, Class<? extends Packet>>> packets =
            new ArrayList<>();


    public Packets() {
        Reflections reflections = new Reflections(Packet.class.getPackage().getName());
        reflections.getSubTypesOf(Packet.class).stream().filter(c -> c.isAnnotationPresent(Type.class)).forEach(c -> {
            Type type = c.getAnnotation(Type.class);
            PacketType packetType = type.type();
            AbstractMap.SimpleEntry<PacketType, Class<? extends Packet>> entry = new
                    AbstractMap.SimpleEntry<>(packetType, c);
            packets.add(entry);
        });
    }

    public PacketType getPacketType(Class<? extends Packet> clasz) {
        for(AbstractMap.SimpleEntry<PacketType, Class<? extends Packet>> entry : packets)
            if(entry.getValue() == clasz)
                return entry.getKey();
        return null;
    }

    public Class<? extends Packet> getPacketClass(PacketType type) {
        for(AbstractMap.SimpleEntry<PacketType, Class<? extends Packet>> entry : packets)
            if(entry.getKey() == type)
                return entry.getValue();
        return null;
    }

    public static Packets getInstance() {
        if(instance == null)
            instance = new Packets();
        return instance;
    }
}
