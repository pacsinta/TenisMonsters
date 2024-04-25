using System;
using UnityEngine;

public enum CourtSquare
{
    HostRight = 0,
    HostLeft = 1,
    ClientRight = 2,
    ClientLeft = 3,
    Out = 4
}

public static class CourtData
{
    private static float X_PlayFieldBoarder = 7.144f;
    private static float Z_PlayFieldBoarder = 20.658f;

    public static CourtSquare GetCurrentCourtSquare(Vector3 position)
    {
        if(IsOut(position)) return CourtSquare.Out;

        if(position.z > 0) // Client Side
        {
            if(position.x > 0)
            {
                return CourtSquare.ClientLeft;
            }
            else
            {
                return CourtSquare.ClientRight;
            }
        }
        else // Host Side
        {
            if(position.x > 0)
            {
                return CourtSquare.HostRight;
            }
            else
            {
                return CourtSquare.HostLeft;
            }
        }
    }
    public static bool IsHostSide(CourtSquare square)
    {
        return square == CourtSquare.HostRight || square == CourtSquare.HostLeft;
    }
    public static bool IsClientSide(CourtSquare square)
    {
        return square == CourtSquare.ClientRight || square == CourtSquare.ClientLeft;
    }

    private static bool IsOut(Vector3 position)
    {
        if(Math.Abs(position.z) > Z_PlayFieldBoarder) return true;
        if(Math.Abs(position.x) > X_PlayFieldBoarder) return true;
        return false;
    }
}
