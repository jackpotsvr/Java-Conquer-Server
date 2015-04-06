package net.co.java.server;

import net.co.java.npc.dialogs.NPCDialogAggregrate;
import org.luaj.vm2.LuaFunction;
import org.luaj.vm2.LuaValue;
import org.luaj.vm2.lib.jse.CoerceJavaToLua;
import org.luaj.vm2.lib.jse.CoerceLuaToJava;

import javax.script.*;
import java.io.File;
import java.nio.file.Files;
import static java.nio.file.Paths.get;

/**
 * Created by Thomas on 05/04/2015.
 */
public class TestLua {
    private ScriptEngine scriptEngine;

    public TestLua() throws Exception {
        final String scriptText = new String(Files.readAllBytes(get("lua/npc/103.lua")));
        final int INITIAL_DIALOG = -1;
        ScriptEngineManager sem = new ScriptEngineManager();
        scriptEngine = sem.getEngineByExtension(".lua");
        CompiledScript script = ((Compilable)scriptEngine).compile(scriptText);

        //System.out.println("Start at: " + System.currentTimeMillis());

        Bindings bindings = scriptEngine.createBindings();
        script.eval(bindings);


        NPCDialogAggregrate npcDialogAggregrate = new NPCDialogAggregrate(null);
        LuaValue aggregrate = CoerceJavaToLua.coerce(npcDialogAggregrate);


        LuaFunction function = (LuaFunction) bindings.get("handle");
        function.call(aggregrate, LuaValue.valueOf(INITIAL_DIALOG));

        npcDialogAggregrate = (NPCDialogAggregrate) CoerceLuaToJava.coerce(aggregrate, NPCDialogAggregrate.class);
        npcDialogAggregrate.spamAllThings();
            //CoerceJavaToLua.coerce()

        //System.out.println("Done at: " + System.currentTimeMillis());

    }

    public static void main(String... args) throws Exception {
        new TestLua();
    }
}
