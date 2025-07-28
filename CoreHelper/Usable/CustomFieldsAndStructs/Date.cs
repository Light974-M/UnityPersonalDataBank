using UnityEngine;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.CoreHelper.Usable.CustomFieldsAndStructs
{
	[System.Serializable]
	public struct Date
	{
		[SerializeField, Tooltip("value of milliseconds passed since the beginning of ages")]
		private int _value;

        public float Seconds
		{
			get
			{
				return _value / 1000f;
			}

			set
			{
				_value = Mathf.RoundToInt(value * 1000);
			}
		}

        public float Minutes
		{
			get
			{
				return Seconds / 60f;
			}

			set
			{
				Seconds = value * 60;
			}
		}

        public float Hours
		{
			get
			{
				return Minutes / 60f;
			}

			set
			{
				Minutes = value * 60;
			}
		}

        public float Days
        {
            get
            {
                return Hours / 24f;
            }

            set
            {
                Hours = value * 24f;
            }
        }

        public float Weeks
        {
            get
            {
                return Days / 7f;
            }

            set
            {
                Days = value * 7f;
            }
        }

        public float Months
        {
            get
            {
                return Days / 7f;
            }

            set
            {
                Days = value * 7f;
            }
        }

        public float Years
        {
            get
            {
                float value = 0;
                bool isBisextil = true;

                for (float i = 0; i < Days; i += isBisextil ? 366 : 365)
                {
                    isBisextil = UPDBBehaviour.IsBisextil((int)value);
                    value += (Days - i) < (isBisextil ? 366 : 365) ? (Days - i) / (isBisextil ? 366f : 365f) : 1;
                }

                return value;
            }

            set
            {
                float daysValue = 0;
                bool isBisextil = true;

                for (int i = 0; i < value; i++)
                {
                    isBisextil = UPDBBehaviour.IsBisextil(i);
                    daysValue += (value - i) < 1 ? (value - i) * (isBisextil ? 366f : 365f) : (isBisextil ? 366f : 365f);
                }

                Days = daysValue;
            }
        }
    } 
}
