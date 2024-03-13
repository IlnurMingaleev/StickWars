using UnityEngine;

public static class Utils
{

    public static GameResources gameResources;

    public static GameResources InitResources()
    {
        Debug.Log("Resources initialized.");
        return gameResources = Resources.Load<GameResources>("GameResources");
    }

    public static GameObject GetUnitById(int itemId)
    {
        return gameResources.items[itemId];
    }
}