using OPSProServer.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPSPro.app.models
{
    public class SelectCard
    {
        public CardResource CardResource { get; }
        public Guid Id { get; }
        public CardSource Source { get; }

        public SelectCard(CardResource cardResource, Guid id, CardSource source)
        {
            CardResource = cardResource;
            Id = id;
            Source = source;
        }

        public SelectCard(SlotCard slotCard) : this(slotCard.Card.CardResource, slotCard.Guid, slotCard.CardActionResource.Source) { }
    }
}
