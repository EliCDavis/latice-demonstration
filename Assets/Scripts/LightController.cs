using UnityEngine;

namespace EliCDavis.Latice
{

    public abstract class LightController
    {

        protected float[] pwmValues;

        protected float[] distanceReadings;

        public LightController(int numLights)
        {
            pwmValues = new float[numLights];
        }

        /// <summary>
        /// Tell the controller what the current distance sensor valuse are.
        /// </summary>
        /// <param name="distances">The distances recorded by different distance sensors</param>
        public void SetSensorValues(float[] distanceReadings)
        {
            this.distanceReadings = distanceReadings;
        }

        /// <summary>
        /// Determine the intensity the lights should be at given the current 
        /// time.
        /// </summary>
        /// <param name="currentTime">The current point in time</param>
        /// <returns>The intensity each light should be at.</returns>
        public abstract float[] GetPWMValues(float currentTime);

        protected static float[] DistanceToPWM(float[] distanceReadings, int numLights, float maxDistance)
        {
            var pwms = new float[numLights];

            float sensorsPerLight = distanceReadings.Length / (float)numLights;

            for (int lightIndex = 0; lightIndex < numLights; lightIndex++)
            {
                var accumlativeSensorIndex = (lightIndex + 1) * sensorsPerLight;

                // If we meet an index exactly...
                if (accumlativeSensorIndex % 1 == 0)
                {
                    var dist = distanceReadings[Mathf.FloorToInt(accumlativeSensorIndex) - 1];
                    pwms[lightIndex] = dist == -666 ? 0 : (1.0f - (dist / maxDistance));
                }
                // The value of this light is split between two distance sensors.
                else if (Mathf.Floor(accumlativeSensorIndex) != Mathf.Floor(lightIndex * sensorsPerLight))
                {
                    var oneIndex = Mathf.FloorToInt(accumlativeSensorIndex) - 1;
                    var dist1 = distanceReadings[oneIndex];

                    var twoIndex = Mathf.FloorToInt(accumlativeSensorIndex);
                    var dist2 = distanceReadings[twoIndex];

                    var dist = (((dist1 == -666 ? 0 : dist1) * .5f) + ((dist2 == -666 ? 0 : dist2) * .5f));

                    pwms[lightIndex] = 1.0f - (dist / maxDistance);
                }
                // We are just dealing with an ordinary index.
                else
                {
                    var dist = distanceReadings[Mathf.FloorToInt(accumlativeSensorIndex)];
                    pwms[lightIndex] = dist == -666 ? 0 : (1.0f - (dist / maxDistance));
                }

            }

            return pwms;
        }

    }

}