using System.Collections.Generic;
using Player.Base.InputHandling;

/*
 * Get fucked lmao
 */
namespace Player.Base.Attacks.Base {
    [System.Serializable]
    public abstract class Attack {
        public virtual int FramesToImpact { get; set; }
        public virtual int FramesToEnd { get; set; }

        public virtual void Enter() { }
        public virtual void Tick() { }
        public virtual void Exit() { }

        public virtual List<int> directionInputs { get; set; }
        
        public virtual bool PunchButtonDown { get; set; }
        public virtual bool KickButtonDown { get; set; }
        public virtual bool slashDown { get; set; }
        public virtual bool heavySlashButtonDown { get; set; }
    }
}