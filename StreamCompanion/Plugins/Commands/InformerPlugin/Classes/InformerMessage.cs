﻿namespace InformerPlugin.Classes;

public class InformerMessage
{
    public string Text { get; set; }
    public int Timeout { get; set; }
    public string[] Aliases { get; set; }
}