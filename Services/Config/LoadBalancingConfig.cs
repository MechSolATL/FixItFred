namespace Services.Config
{
    public class LoadBalancingWeights
    {
        public double Load { get; set; }
        public double Distance { get; set; }
        public double Sla { get; set; }
        public double Priority { get; set; }
        public double History { get; set; }
        public double Fairness { get; set; }
    }

    public class LoadBalancingConfig
    {
        public LoadBalancingWeights Weights { get; set; } = new();
        public bool HardFailOnMissingSkill { get; set; } = true;
    }
}
