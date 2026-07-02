using System.Collections.Generic;
using Player.Base.Attacks.Base.Info;
using Player.Base.Attacks.Base.Validator.Base;

namespace Player.Base.Attacks.Base {
    public class Attack {
        //Input Requirements
        public AttackStance requiredStance = AttackStance.Any;
        public List<int> directionInputs;
        public ButtonType button = ButtonType.Punch;
        
        //Frame Data
        public int startup;
        public int active;
        public int recovery;

        public int cancelWindowStart;
        public int cancelWindowEnd;
        
        //Hit Data
        public List<HitboxDefinition> hitboxes;
        
        //animation
        public string animationName;
        
        //events
        public List<AttackEvent> events;
    }
}