package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class ChatResponse {
    @SerializedName(value = "answer", alternate = {"Answer", "response", "Response", "message", "Message", "content", "Content"})
    private String answer;

    public String getAnswer() {
        return answer != null ? answer : "";
    }

    public void setAnswer(String answer) {
        this.answer = answer;
    }
}
