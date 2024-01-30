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
        public bool Selected { get; set; }
        public CardSource Source { get; set; }

        public SelectCard(CardResource cardResource, Guid id, bool selected, CardSource source)
        {
            CardResource = cardResource;
            Id = id;
            Selected = selected;
        }

        public SelectCard(CardResource cardResource, Guid id, CardSource source) : this(cardResource, id, false, source) { }

        public SelectCard(SlotCard slotCard) : this(slotCard.Card.CardResource, slotCard.Guid, false, slotCard.CardActionResource.Source) { }
    }
}
