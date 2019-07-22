using UnityEngine;

namespace EliCDavis.Latice
{

    public class ProximityBlinkingController : LightController
    {

        private float lastTime = 0;

        /// <summary>
        /// The rate in which the LEDs are able to change their light intensity
        /// </summary>
        private float movementSpeed = 0.3f;

        public ProximityBlinkingController(int numLights) : base(numLights) { }

        public override float[] GetPWMValues(float newTime)
        {
            var deltaTime = newTime - lastTime;

            var newValues = DistanceToPWM(distanceReadings, pwmValues.Length, 2);
            for (int i = 0; i < newValues.Length; i++)
            {
                newValues[i] = Mathf.Clamp01(newValues[i] + (newValues[i] * Mathf.Sin((newTime * 5) + (i * 10))));
            }

            for (int i = 0; i < pwmValues.Length; i++)
            {
                if (Mathf.Abs(newValues[i] - pwmValues[i]) < movementSpeed * deltaTime)
                {
                    pwmValues[i] = newValues[i];
                } else
                {
                    pwmValues[i] = pwmValues[i] + ((movementSpeed * deltaTime) * (newValues[i] > pwmValues[i] ? 1 : -1)); 
                }
            }

            lastTime = newTime;
            return pwmValues;
        }

    }

}