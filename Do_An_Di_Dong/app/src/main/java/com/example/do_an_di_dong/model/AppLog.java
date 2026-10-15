package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class AppLog {
    @SerializedName(value = "logId", alternate = {"LogId", "id", "Id"})
    private long logId;

    @SerializedName(value = "logLevel", alternate = {"LogLevel", "level", "Level"})
    private String logLevel;

    @SerializedName(value = "logger", alternate = {"Logger"})
    private String logger;

    @SerializedName(value = "message", alternate = {"Message"})
    private String message;

    @SerializedName(value = "exception", alternate = {"Exception"})
    private String exception;

    @SerializedName(value = "thread", alternate = {"Thread"})
    private String thread;

    @SerializedName(value = "createdAt", alternate = {"CreatedAt", "timestamp", "Timestamp"})
    private String createdAt;

    public long getLogId() { return logId; }
    public String getLogLevel() { return logLevel; }
    public String getLogger() { return logger; }
    public String getMessage() { return message; }
    public String getException() { return exception; }
    public String getThread() { return thread; }
    public String getCreatedAt() { return createdAt; }
}
