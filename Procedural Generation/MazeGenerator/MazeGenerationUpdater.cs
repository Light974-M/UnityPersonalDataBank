using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UPDB.ProceduralGeneration.MazeGenerator
{
	public class MazeGenerationUpdater
	{
        private GenerationUpdateType _updateType = GenerationUpdateType.Instant;

        private Cell _toCallCell = null;

        private bool _value = false;

        #region Public API

        public GenerationUpdateType UpdateType
        {
            get { return _updateType; }
            set { _updateType = value; }
        }

        public Cell ToCallCell
        {
            get { return _toCallCell; }
            set { _toCallCell = value; }
        }

        public bool Value => _value;

        public bool Read
        {
            get
            {
                if (_value)
                    OnTriggerReadBehaviour();

                bool value = _value;
                _value = false;
                return value;
            }
        } 

        #endregion

        public void Activate()
        {
            _value = true;
        }

        public void Disarm()
        {
            _value = false;
        }

        private void OnTriggerReadBehaviour()
        {

        }
    } 

    public enum GenerationUpdateType
    {
        Instant,
        FrameRate,
        FixedSecond,
        ManualUpdate,
    }
}
