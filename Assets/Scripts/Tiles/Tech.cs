
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

    public static TechStruct ConvertToStruct(Tech tech){
        return new TechStruct{
            societyLevel = tech.societyLevel,
            industryLevel = tech.industryLevel,
            militaryLevel = tech.militaryLevel
        };
    }
    public static Tech ReturnToClass(TechStruct ts){
        return new Tech(){
            societyLevel = ts.societyLevel,
            industryLevel = ts.industryLevel,
            militaryLevel = ts.militaryLevel
        };
    }
}

public struct TechStruct{
    public int societyLevel;
    public int industryLevel;
    public int militaryLevel;
}
