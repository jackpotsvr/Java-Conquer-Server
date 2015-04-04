package net.co.java.tasks;

import java.util.concurrent.TimeUnit;

/**
 * Created by Thomas on 03/04/2015.
 */
public abstract class TickTask {
    public abstract Runnable getRunnable();

    public abstract long getPeriod();

    /**
     * @return By default it returns the value of getPeriod();
     * but can be overridden if another initialDelay is demanded.
     */
    public long getInitialDelay() {
        return getPeriod();
    }

    /**
     * @return By default it will return milliseconds, but can
     *  be overridden if another TimeUnit is preferred for the task.
     */
    public TimeUnit getTimeUnit() {
        return TimeUnit.MILLISECONDS;
    }
}
