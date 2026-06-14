using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Requests.UpdateRequests;

public sealed class UpdateCollectionOrderRequest
{
    [Required] public Dictionary<int, int> CollectionIdToNewOrderPosition { get; set; } = new();
}
