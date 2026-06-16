using UnityEngine;

public enum AiDistanceType
{
    CloseRange,       
    TurretRange,      
    LowLongRange,     
    MediumLongRange,  
    LongLongRange     
}

public struct AiRangeData
{

    public AiDistanceType Type;
    public float MinDistance;
    public float MaxDistance;

    public AiRangeData(AiDistanceType type, float minDistance, float maxDistance)
    {
        Type = type;
        MinDistance = minDistance;
        MaxDistance = maxDistance;
    }
}

public static class AiTypePicker
{
    const int closeRange = 20;
    const int longRange = 100;

    public static AiRangeData PickAiType(int score, float turretRange)
    {
        if (score >= -10 && score <= -7)
        {
            return CloseRange();
        }

        if (score > -7 && score < -3)
        {
            return TurretRange(turretRange);
        }

        if (score >= -3 && score <= 3)
        {
            return LowLongRange(turretRange);
        }

        if (score > 3 && score < 7)
        {
            return MediumLongRange(turretRange);
        }

        if (score >= 7 && score <= 10)
        {
            return LongLongRange();
        }

        Debug.LogError($"Invalid AI score: {score}");
        return CloseRange();
    }

    private static AiRangeData CloseRange()
    {
        return new AiRangeData(
            AiDistanceType.CloseRange,
            0f,
            closeRange
        );
    }

    private static AiRangeData TurretRange(float turretRange)
    {
        return new AiRangeData(
            AiDistanceType.TurretRange,
            closeRange,
            turretRange
        );
    }

    private static AiRangeData LowLongRange(float turretRange)
    {
        return new AiRangeData(
            AiDistanceType.LowLongRange,
            closeRange,
            turretRange
        );
    }

    private static AiRangeData MediumLongRange(float turretRange)
    {
        return new AiRangeData(
            AiDistanceType.MediumLongRange,
            closeRange,
            turretRange
        );
    }

    private static AiRangeData LongLongRange()
    {
        return new AiRangeData(
            AiDistanceType.LongLongRange,
            closeRange,
            longRange
        );
    }
}