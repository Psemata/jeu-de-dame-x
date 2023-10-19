using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JeuDeDameX.Model
{
    public interface IObserver
    {
        // Receive update from subject
        void Update();
        // Receive an update from the subject saying that the turn is changing
        void ChangeTurn();
    }
}
