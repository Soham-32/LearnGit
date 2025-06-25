namespace AtCommon.Dtos.BusinessOutcomes.MeetingNotes
{
    public class UpdateBusinessOutcomeMeetingNotesActionItemStatusRequest
    {
        public bool IsCompleted { get; set; }
        public int ActionItemId { get; set; }
        
    }
}