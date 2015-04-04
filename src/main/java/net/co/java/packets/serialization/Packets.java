package net.co.java.packets.serialization;

import net.co.java.packets.GeneralDataPacket;
import net.co.java.packets.Packet;
import net.co.java.packets.PacketHandler;
import net.co.java.packets.PacketType;
import net.co.java.packets.packethandlers.NoPacketHandler;
import org.reflections.Reflections;

import java.lang.reflect.InvocationTargetException;
import java.util.AbstractMap;

import java.util.ArrayList;
import java.util.List;

public class Packets {
    private static Packets instance;

    private List<AbstractMap.SimpleEntry<PacketType, Class<? extends Packet>>> packets =
            new ArrayList<>();


    public Packets() {
        Reflections reflections = new Reflections(GeneralDataPacket.class.getPackage().getName());
        /*
        reflections.getSubTypesOf(Packet.class).stream().filter(c -> c.isAnnotationPresent(Type.class)).forEach(c -> {
            Type type = c.getAnnotation(Type.class);
            PacketType packetType = type.type();
            AbstractMap.SimpleEntry<PacketType, Class<? extends Packet>> entry = new
                    AbstractMap.SimpleEntry<>(packetType, c);
            packets.add(entry);
        }); */
        for(Class<? extends Packet> c : reflections.getSubTypesOf(Packet.class)) {
            if(!c.isAnnotationPresent(Type.class))
                break;
            Type type = c.getAnnotation(Type.class);
            PacketType packetType = type.type();
            AbstractMap.SimpleEntry<PacketType, Class<? extends Packet>> entry = new
                    AbstractMap.SimpleEntry<>(packetType, c);
            packets.add(entry);
        }
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

    public static PacketHandler getHandlerStrategy(Packet packet) {
        try {
            Class<?> clasz = Packets.getInstance().getPacketClass(packet.getType());
            if (clasz.isAnnotationPresent(Bidirectional.class)){
                Bidirectional bidirectional = clasz.getAnnotation(Bidirectional.class);
                return bidirectional.handler().getConstructor(Packet.class).newInstance(packet);
            } else if (clasz.isAnnotationPresent(Incoming.class)) {
                Incoming incoming = clasz.getAnnotation(Incoming.class);
                return incoming.handler().getConstructor(Packet.class).newInstance(packet);
            } else {
                return NoPacketHandler.class.getConstructor(Packet.class).newInstance(packet);
            }
        } catch (InstantiationException | IllegalAccessException |
                IllegalArgumentException | InvocationTargetException |
                NoSuchMethodException | SecurityException e) {
            throw new RuntimeException("Couldn't find the Packet's handler.");
        }
    }
}
