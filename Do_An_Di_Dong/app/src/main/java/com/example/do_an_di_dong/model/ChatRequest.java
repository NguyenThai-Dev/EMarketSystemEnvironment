package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class ChatRequest {
    @SerializedName(value = "prompt", alternate = {"Prompt", "message", "Message", "question", "Question"})
    private String prompt;

    @SerializedName(value = "userName", alternate = {"UserName", "username", "Username", "user", "User"})
    private String userName;

    public ChatRequest(String prompt, String userName) {
        this.prompt = prompt;
        this.userName = userName;
    }

    public String getPrompt() {
        return prompt;
    }

    public void setPrompt(String prompt) {
        this.prompt = prompt;
    }

    public String getUserName() {
        return userName;
    }

    public void setUserName(String userName) {
        this.userName = userName;
    }
}
