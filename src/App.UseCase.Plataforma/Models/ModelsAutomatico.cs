﻿namespace app.plataforma;


public class UserAutomatic
{
    public string? Username { get; set; }
    public string? ConnectionId { get; set; }
    public bool InCall { get; set; }
}

public class ConnectionAutomatic
{
    public List<UserAutomatic>? Users { get; set; }
}
public class CallAutomatic
{
    public UserAutomatic? From { get; set; }
    public UserAutomatic? To { get; set; }
    public DateTime CallStartTime { get; set; }
}


