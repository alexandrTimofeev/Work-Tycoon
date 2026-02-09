using System.Collections;
using UnityEngine;

public static class TB
{

    private static string ClR => "white";
    private static string ClM => "green";

    public static string R => $"<color={ClR}>R</color>";
    public static string M => $"<color={ClM}>$</color>";
    public static string X => $"x";
    public static string PAdd => $"x+";
    public static string PUp => $"xx";
    public static string S => $"/s ";
    public static string Rs => $"{R}<color={ClR}>{S}</color>";
    public static string Ms => $"{M}<color={ClM}>{S}</color>";
    public static string Dots => "<size=4>...</size>";

    public static string PowerUp => "PowerUp";
    public static string SpeedUp => "SpeedUp";

}