
using UnityEngine;

public class Tech
{
    public int societyLevel;
    public int industryLevel;
    public int militaryLevel;

    public static bool CheckSimilarity(Tech a, Tech b, int minThreshold = 1){
        bool similarSociety = Mathf.Abs(a.societyLevel - b.societyLevel) < minThreshold;
        bool similarIndustry = Mathf.Abs(a.industryLevel - b.industryLevel) < minThreshold;
        bool similarMilitary = Mathf.Abs(a.militaryLevel - b.militaryLevel) < minThreshold;

        return similarIndustry && similarSociety && similarMilitary;
    }
}
