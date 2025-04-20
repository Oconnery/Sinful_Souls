using UnityEngine;

public class SinfulUtility{
    private double RoundToSignificantDigits(double d, int digits) {
        if (d == 0) {
            return 0;
        }

        double scale = System.Math.Pow(10, System.Math.Floor(System.Math.Log10(System.Math.Abs(d))) + 1);
        return scale * System.Math.Round(d / scale, digits);
    }

    private ulong CalculateDifference(ulong a, ulong b) {
        if (a > b){
            return a-b;
        } else {
            return b-a;
        }
    }
}
