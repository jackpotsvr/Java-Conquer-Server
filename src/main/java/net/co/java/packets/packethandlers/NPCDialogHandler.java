package net.co.java.packets.packethandlers;

import net.co.java.entity.Entity;
import net.co.java.entity.NPC;
import net.co.java.entity.Player;
import net.co.java.npc.dialogs.NPCDialogAggregrate;
import net.co.java.packets.NPCDialogPacket;
import net.co.java.packets.NPCInitialPacket;
import net.co.java.packets.Packet;
import net.co.java.packets.serialization.PacketSerializer;
import net.co.java.server.GameServerClient;
import org.luaj.vm2.LuaFunction;
import org.luaj.vm2.LuaValue;
import org.luaj.vm2.lib.jse.CoerceJavaToLua;
import org.luaj.vm2.lib.jse.CoerceLuaToJava;
import sun.font.Script;

import javax.script.*;
import java.awt.*;
import java.io.IOException;
import java.nio.file.Files;

import static java.nio.file.Paths.get;

/**
 * Created by Thomas on 06/04/2015.
 */
public class NPCDialogHandler extends AbstractPacketHandler {
    private ScriptEngine scriptEngine;
    private NPCDialogAggregrate aggregrate;

    public NPCDialogHandler(Packet packet) {
        super(packet);
    }

    @Override
    public void handle(GameServerClient client) {
        Player hero = client.getPlayer();
        if(packet instanceof NPCInitialPacket) {
            NPCInitialPacket nip = (NPCInitialPacket) packet;
            NPC npc = null;

            for(Entity e : 	client.getPlayer().getLocation().getMap().getEntities())
                if(e.getIdentity() == nip.getNpcUID())
                    npc = (NPC)e;
            if(npc == null) return;
            aggregrate = new NPCDialogAggregrate(npc);
            hero.setActiveDialog(aggregrate);
        } else if(packet instanceof NPCDialogPacket) {
            NPCDialogPacket ndp = (NPCDialogPacket) packet;
            aggregrate = hero.getActiveDialog();
            if(ndp.getDialog() == 255) return; 
            aggregrate.setCurrentDialogNumber(ndp.getDialog());
        } else return;

        if(aggregrate == null) return;
        workNPCScript(client);
    }

    public void workNPCScript(GameServerClient client) {
        try {
            final String scriptText = new String(Files.readAllBytes(get("lua/npc/103.lua")));
            ScriptEngineManager sem = new ScriptEngineManager();
            scriptEngine = sem.getEngineByExtension(".lua");
            CompiledScript script = ((Compilable)scriptEngine).compile(scriptText);

            Bindings bindings = scriptEngine.createBindings();
            script.eval(bindings);

            LuaValue luaAggregrate = CoerceJavaToLua.coerce(aggregrate);


            LuaFunction function = (LuaFunction) bindings.get("handle");
            function.call(luaAggregrate);
            NPCDialogAggregrate npcDialogAggregrate =
                    (NPCDialogAggregrate) CoerceLuaToJava.coerce(luaAggregrate, NPCDialogAggregrate.class);

            for(NPCDialogAggregrate.NPCDialog d :npcDialogAggregrate.getDialogs()) {
                new PacketSerializer(d.getPacket()).serialize().send(client);
            }
        } catch (ScriptException e) {
            e.printStackTrace();
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

}
