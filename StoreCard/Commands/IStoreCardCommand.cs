using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreCard.Commands;

internal interface IStoreCardCommand
{
    public void Execute();
}