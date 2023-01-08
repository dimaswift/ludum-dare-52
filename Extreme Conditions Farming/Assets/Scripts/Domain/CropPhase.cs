namespace ECF.Domain
{
    public enum CropPhase
    {
        Seed = 0,
        Sprout = 1,
        Bud = 2,
        Flower = 3,
        Green = 4,
        Unripe = 5,
        Ripe = 6,
        Overripe = 7,
        Rotten = 8
    }

    public enum BedStatus
    {
        Locked = 0,
        Empty = 1,
        Planted = 2
    }

    public enum Genetics
    {
        Poor = 0,
        Average = 2,
        Good = 3,
        Excellent = 4,
        Mutant = 5
    }
}