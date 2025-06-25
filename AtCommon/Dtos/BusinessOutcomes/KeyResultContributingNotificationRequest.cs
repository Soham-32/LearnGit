using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtCommon.Dtos.BusinessOutcomes
{
    public class KeyResultContributingNotificationRequest
    {
        public int Id { get; set; }
        public double AcceptedOnNotificationValue { get; set; }
        public KeyResultContributingNotificationStatus NotificationStatusId { get; set; }
        public Guid ParentKeyResultUid { get; set; }
        public Guid ChildKeyResultUid { get; set; }
    }
    public enum KeyResultContributingNotificationStatus
    {
        Active,
        Accepted,
        Discarded,
        Overwritten,
        Deleted
    }
}
