using System;
using System.Collections.Generic;
using System.Device.Spi;
using System.Drawing;
using System.Threading;
using Iot.Device.Graphics;
using Iot.Device.Hcsr04;
using Iot.Device.Ws28xx;
using UnitsNet;

namespace garageParking {
    class Program {
        static void Main (string[] args) {
            Console.WriteLine ("Hello World!");

            //Ws28xx neo = new Ws2812b(spi, Count);
            sense ();
        }

        static void sense () {
            Console.WriteLine ("Hello Hcsr04 Sample!");

            // Configure the count of pixels
            const int Count = 8;
            // Console.Clear();

            SpiConnectionSettings settings = new SpiConnectionSettings (0, 0) {
                ClockFrequency = 2_400_000,
                Mode = SpiMode.Mode0,
                DataBitLength = 8
            };

            using SpiDevice spi = SpiDevice.Create (settings);
            Ws28xx neo = new Ws2812b (spi, Count);
            Rainbow (neo, Count);

            Console.CancelKeyPress += (o, e) => {
                BitmapImage img = neo.Image;
                img.Clear ();
                neo.Update ();
                Console.Clear ();
            };

using (var sonar = new Hcsr04 (4, 17)) {
            while (true) {
                
                    try {

                        if (sonar.TryGetDistance (out Length distance)) {

                            Console.WriteLine ($"Distance: {distance.Centimeters} cm");
                            if (distance.Centimeters >=60.1) {
                                // Green
                                Console.WriteLine ("GREEN");
                                ColorWipe (neo, Color.Green, Count);
                            } else if (distance.Centimeters <= 60 && distance.Centimeters >= 30.1) {
                                // Yellow
                                Console.WriteLine ("Yellow");
                                ColorWipe (neo, Color.Yellow, Count);
                            } else if (distance.Centimeters <= 30) {
                                // RED
                                Console.WriteLine ("Red");
                                ColorWipe (neo, Color.Red, Count);
                                // Go to Sleep
                                Thread.Sleep(5000);
                                BitmapImage img = neo.Image;
                                img.Clear ();
                                neo.Update ();
                                Console.Clear ();

                                bool sleep = true;
                                while(sleep)
                                {
                                    // Sleeping
                                    if (sonar.TryGetDistance (out Length distanceSleep)) 
                                    {
                                        if (distanceSleep.Centimeters >= 30.1)
                                        {
                                            sleep = false;
                                        }
                                        else{
                                            Console.WriteLine ($"Sleeping Distance: {distanceSleep.Centimeters} cm");
                                            Thread.Sleep(5000);
                                        }
                                    }
                                    
                                }
                            }
                        } else {
                            Console.WriteLine ("Error reading sensor");
                        }

                        Thread.Sleep (1000);
                    } catch (Exception ex) {
                        // TODO 
                    }

                }
            }
        }

        static void Rainbow (Ws28xx neo, int count, int iterations = 1) {
            BitmapImage img = neo.Image;
            for (var i = 0; i < 255 * iterations; i++) {
                for (var j = 0; j < count; j++) {
                    img.SetPixel (j, 0, Wheel ((i + j) & 255));
                }

                neo.Update ();
            }
        }

        static void ColorWipe (Ws28xx neo, Color color, int count) {
            BitmapImage img = neo.Image;
            for (var i = 0; i < count; i++) {
                img.SetPixel (i, 0, color);
                neo.Update ();
            }
        }
        static Color Wheel (int position) {
            if (position < 85) {
                return Color.FromArgb (position * 3, 255 - position * 3, 0);
            } else if (position < 170) {
                position -= 85;
                return Color.FromArgb (255 - position * 3, 0, position * 3);
            } else {
                position -= 170;
                return Color.FromArgb (0, position * 3, 255 - position * 3);
            }
        }
    }
}
