using System;

public static class Global_Population_Viewer{
    public static event Func<ulong> OnTotalEvilPopulationRequest;
    public static event Func<ulong> OnTotalGoodPopulationRequest;
    public static event Func<ulong> OnTotalNeutralPopulationRequest;


    public static ulong GetTotalEvilPopulation() {
        ulong evilPopulation = 0;

        foreach(var del in OnTotalEvilPopulationRequest.GetInvocationList()){
            evilPopulation += ((Func<ulong>)del).Invoke();
        }

        return evilPopulation;
    }

    public static ulong GetTotalGoodPopulation() {
        ulong goodPopulation = 0;

        foreach (var del in OnTotalGoodPopulationRequest.GetInvocationList()) {
            goodPopulation += ((Func<ulong>)del).Invoke();
        }

        return goodPopulation;
    }

    public static ulong GetTotalNeutralPopulation() {
        ulong neutralPopulation = 0;

        foreach (var del in OnTotalNeutralPopulationRequest.GetInvocationList()) {
            neutralPopulation += ((Func<ulong>)del).Invoke();
        }

        return neutralPopulation;
    }
}
