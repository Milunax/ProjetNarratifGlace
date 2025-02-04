[System.Serializable]
public struct PROGRESSION
{
    public int day;
    public int inDay;
    public int narrativeIALevel;

    public PROGRESSION(int nDay, int ninDay, int nNarIALevel)
    {
        day = nDay;
        inDay = ninDay;
        narrativeIALevel = nNarIALevel;
    }
}