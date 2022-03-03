using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JominiGame
{
    public class BaseGameObject
    {

        public GameClock Clock;
        public IdGenerator IDGen;
        public HexMapGraph GameMap;

        public BaseGameObject(GameClock Clock, IdGenerator IDGen, HexMapGraph GameMap)
        {
            this.Clock = Clock;
            this.IDGen = IDGen;
            this.GameMap = GameMap;
        }

    }
}
