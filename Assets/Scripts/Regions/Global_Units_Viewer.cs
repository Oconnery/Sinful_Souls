using System;

public static class Global_Units_Viewer{
    public static event Func<ushort> OnDeployedEvilAgentsRequest;
    public static event Func<ushort> OnDeployedEvilSecondaryUnitsRequest;
    public static event Func<ushort> OnDeployedGoodAgentsRequest;
    public static event Func<ushort> OnDeployedGoodSecondaryUnitsRequest;

    public static ushort GetDeployedEvilAgents() {
        ushort deployedDemons = 0;

        foreach (var del in OnDeployedEvilAgentsRequest.GetInvocationList()) {
            deployedDemons += ((Func<ushort>)del).Invoke();
        }

        return deployedDemons;
    }

    public static ushort GetDeployedEvilSecondaryUnits() {
        ushort deployedBanshees = 0;

        foreach (var del in OnDeployedEvilSecondaryUnitsRequest.GetInvocationList()) {
            deployedBanshees += ((Func<ushort>)del).Invoke();
        }

        return deployedBanshees;
    }

    public static ushort GetDeployedGoodAgents() {
        ushort deployedAngels = 0;

        foreach (var del in OnDeployedGoodAgentsRequest.GetInvocationList()) {
            deployedAngels += ((Func<ushort>)del).Invoke();
        }

        return deployedAngels;
    }

    public static ushort GetDeployedGoodSecondaryAgents() {
        ushort deployedInquisitors = 0;

        foreach (var del in OnDeployedGoodSecondaryUnitsRequest.GetInvocationList()) {
            deployedInquisitors += ((Func<ushort>)del).Invoke();
        }

        return deployedInquisitors;
    }

}
