package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class SystemEvent {
    @SerializedName(value = "id", alternate = {"Id", "eventId", "EventId"})
    private String id;

    @SerializedName(value = "title", alternate = {"Title", "name", "Name"})
    private String title;

    @SerializedName(value = "description", alternate = {"Description", "message", "Message"})
    private String description;

    @SerializedName(value = "eventType", alternate = {"EventType", "type", "Type"})
    private String eventType;

    @SerializedName(value = "timestamp", alternate = {"Timestamp", "createdAt", "CreatedAt"})
    private String timestamp;

    public String getId() { return id; }
    public String getTitle() { return title; }
    public String getDescription() { return description; }
    public String getEventType() { return eventType; }
    public String getTimestamp() { return timestamp; }
}
