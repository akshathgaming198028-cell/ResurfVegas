namespace com.kiloo.awards
{
	public class AwardStatGoal : AwardGoal
	{
		public Stat stat { get; set; }

		public AwardStatGoal()
		{
			base.hasIcon = false;
		}
	}
}
