/* ==========================================================================================================================================
 * Copyright (c) 2017 Kevin Dai
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files
 * (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge,
 * publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 * 
 *  (1) The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
 * LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 * 
 * ==========================================================================================================================================
 * 
 * This is the core of all the things. Everything is in here. Yup.
 * The code below will convert the SVG in such a way that your teacher will suspect nothing.
 * Well... expect that all your arcs have this property: xRadius = yRadius
 * Which is unfortunate. If you are viewing this on GitHub, I feel sorry for you. I folded the code so nicely in the IDE.
 * If you are viewing this inside of Visual Studio, yay! You get all the awesome code folding.
 */

using Svg;
using Svg.Pathing;

using System;
using System.Linq;
using System.Numerics;
using System.Text;

namespace SVGtoTuring
{
    /// <summary>
    /// Summary goes here. Self explanatory!
    /// </summary>
    public static class ParseSVG
    {
        #region Global Variables

        static int gH = 0; //Global height
        static int gW = 0; //Global width
        static string[] turingColors = new string[]
        {
            "gray",
            "red",
            "orange",
            "brown",
            "yellow",
            "green",
            "lime",
            "turquoise",
            "cyan",
            "sky",
            "blue",
            "violet",
            "magenta",
            "pink"
        };

        #endregion

        #region Main Method(s)

        /// <summary>
        /// Converts an SVGDocument into lines of Turing Code. Syntax in OpenTuring v2.0
        /// </summary>
        /// <param name="file">The SvgDocument</param>
        /// <returns>Turing Code</returns>
        public static string toTuring(SvgDocument file)
        {
            gH = (int)file.Height.Value;
            gW = (int)file.Width.Value;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("setscreen (\"graphics: {0}; {1}\")", gW, gH));
            sb.AppendLine("var c1: int % Stroke colour always");
            sb.AppendLine("var c2: int % Fill colour always");

            // Reduce the number of unnecessary repetitions of the usage of c1 & c2 
            string cacheC1 = "";
            string cacheC2 = "";

            // Loop through children and find the type of the SVG Element
            foreach (var c in file.Children)
            {

                #region Lines
                if (c is SvgLine) //Line
                {
                    var line = c as SvgLine;
                    SvgColourServer sv = line.Stroke as SvgColourServer;

                    if (cacheC1 != string.Format("c1 := RGB.AddColour({0}, {1}, {2})", sv.Colour.R / 255f, sv.Colour.G / 255f, sv.Colour.B / 255f))
                    {
                        cacheC1 = string.Format("c1 := RGB.AddColour({0}, {1}, {2})", sv.Colour.R / 255f, sv.Colour.G / 255f, sv.Colour.B / 255f);
                        sb.AppendLine(cacheC1);
                    }

                    if (sv.ToString() != "none")
                        sb.AppendLine(drawLine((int)line.StartX.Value, (int)line.StartY.Value, (int)line.EndX.Value, (int)line.EndY.Value, (int)line.StrokeWidth.Value, getColor(sv.Colour.Name, false)));
                }
                #endregion

                #region Rectangles
                else if (c is SvgRectangle) //Rectangle
                {
                    var rect = c as SvgRectangle;
                    int x1 = (int)rect.X.Value;
                    int y1 = gH - ((int)rect.Y.Value - (int)rect.Height.Value);
                    int x2 = x1 + (int)rect.Width.Value;
                    int y2 = y1 + (int)rect.Height.Value;

                    var sColor = rect.Stroke as SvgColourServer;
                    var fColor = rect.Fill as SvgColourServer;

                    // Get the colour and decide if we should use RGB or Turing (text) colours
                    if ((sColor.ToString() != "none") && !isTuringColor(sColor.Colour.ToString()))
                    {
                        if (cacheC1 != string.Format("c1 := RGB.AddColour({0}, {1}, {2})", sColor.Colour.R / 255f, sColor.Colour.G / 255f, sColor.Colour.B / 255f))
                        {
                            cacheC1 = string.Format("c1 := RGB.AddColour({0}, {1}, {2})", sColor.Colour.R / 255f, sColor.Colour.G / 255f, sColor.Colour.B / 255f);
                            sb.AppendLine(cacheC1);
                        }
                    }

                    if ((fColor.ToString() != "none") && !isTuringColor(fColor.Colour.ToString()))
                    {
                        if (cacheC2 != string.Format("c2 := RGB.AddColour({0}, {1}, {2})", fColor.Colour.R / 255f, fColor.Colour.G / 255f, fColor.Colour.B / 255f))
                        {
                            cacheC2 = string.Format("c2 := RGB.AddColour({0}, {1}, {2})", fColor.Colour.R / 255f, fColor.Colour.G / 255f, fColor.Colour.B / 255f);
                            sb.AppendLine(cacheC2);
                        }
                    }

                    // Check and apply the stroke
                    if ((rect.StrokeWidth.Value > 0) && (sColor.ToString() != "none") && (!sColor.Colour.IsEmpty) && (sColor.Colour.A > 0))
                    {
                        int width = (int)rect.StrokeWidth.Value;
                        sb.AppendLine(drawFillOval(x1 - width, y1 - width, x2 + width, y2 + width, getColor(sColor.Colour.ToString(), false)));
                    }

                    // Check if it is filled
                    if ((fColor.ToString() != "none") && (!fColor.Colour.IsEmpty) && (fColor.Colour.A > 0))
                        sb.AppendLine(drawFillBox(x1, y1, x2, y2, getColor(fColor.Colour.Name, true)));
                    else
                        sb.AppendLine(drawBox(x1, y1, x2, y2, getColor(sColor.Colour.Name, false)));

                } //End if
                #endregion

                #region Paths
                // Thanks to Nathan Lo for helping me in this bit of horror
                // A sh*t ton of math is about crash down on you
                else if (c is SvgPath) // F*cking arcs
                {
                    // SVG takes 2 points and 1 radius to draw arcs
                    // Turing takes in 2 angles, 1 radius and 1 center point to draw it's arcs
                    // See the problem? I see it....
                    var path = c as SvgPath;
                    foreach (var p in path.PathData)
                    {
                        #region Arcs
                        if (p is SvgArcSegment)
                        {
                            var arc = p as SvgArcSegment;
                            //Assumption made about RadiusX = RadiusY, as the SVG program I used only supported circular arcs
                            //So if you want to draw elliptical arcs, you are fucked. And I don't care anymore
                            double cX, cY, sweepFrom, sweepTo;
                            if ((arc.Sweep == SvgArcSweep.Positive) && (arc.Size == SvgArcSize.Large)) //Deal with the SVG arc flags
                            {
                                cX = nCenterX(arc.Start.X, gH - arc.Start.Y, arc.End.X, gH - arc.End.Y, arc.RadiusX);
                                cY = nCenterY(arc.Start.X, gH - arc.Start.Y, arc.End.X, gH - arc.End.Y, arc.RadiusX);

                                sweepTo = 180 * Math.Atan2(Math.Abs(cY - gH + arc.Start.Y), Math.Abs(arc.Start.X - cX)) / Math.PI;
                                sweepFrom = 180 * Math.Atan2(Math.Abs(cY - gH + arc.End.Y), Math.Abs(arc.End.X - cX)) / Math.PI;

                                sweepTo = adjustAngle(sweepTo, gH - arc.Start.Y - cY, arc.Start.X - cX);
                                sweepFrom = adjustAngle(sweepFrom, gH - arc.End.Y - cY, arc.End.X - cX);
                            }

                            else if ((arc.Sweep == SvgArcSweep.Positive) && (arc.Size == SvgArcSize.Small))
                            {
                                cX = pCenterX(arc.Start.X, gH - arc.Start.Y, arc.End.X, gH - arc.End.Y, arc.RadiusX);
                                cY = pCenterY(arc.Start.X, gH - arc.Start.Y, arc.End.X, gH - arc.End.Y, arc.RadiusX);

                                sweepTo = 180 * Math.Atan2(Math.Abs(cY - gH + arc.Start.Y), Math.Abs(arc.Start.X - cX)) / Math.PI;
                                sweepFrom = 180 * Math.Atan2(Math.Abs(cY - gH + arc.End.Y), Math.Abs(arc.End.X - cX)) / Math.PI;

                                sweepTo = adjustAngle(sweepTo, gH - arc.Start.Y - cY, arc.Start.X - cX);
                                sweepFrom = adjustAngle(sweepFrom, gH - arc.End.Y - cY, arc.End.X - cX);
                            }

                            else if ((arc.Sweep == SvgArcSweep.Negative) && (arc.Size == SvgArcSize.Small))
                            {
                                cX = nCenterX(arc.Start.X, gH - arc.Start.Y, arc.End.X, gH - arc.End.Y, arc.RadiusX);
                                cY = nCenterY(arc.Start.X, gH - arc.Start.Y, arc.End.X, gH - arc.End.Y, arc.RadiusX);

                                sweepFrom = 180 * Math.Atan2(Math.Abs(cY - gH + arc.Start.Y), Math.Abs(arc.Start.X - cX)) / Math.PI;
                                sweepTo = 180 * Math.Atan2(Math.Abs(cY - gH + arc.End.Y), Math.Abs(arc.End.X - cX)) / Math.PI;

                                sweepFrom = adjustAngle(sweepFrom, gH - arc.Start.Y - cY, arc.Start.X - cX);
                                sweepTo = adjustAngle(sweepTo, gH - arc.End.Y - cY, arc.End.X - cX);
                            }

                            else
                            {
                                cX = pCenterX(arc.Start.X, gH - arc.Start.Y, arc.End.X, gH - arc.End.Y, arc.RadiusX);
                                cY = pCenterY(arc.Start.X, gH - arc.Start.Y, arc.End.X, gH - arc.End.Y, arc.RadiusX);

                                sweepFrom = 180 * Math.Atan2(Math.Abs(cY - gH + arc.Start.Y), Math.Abs(arc.Start.X - cX)) / Math.PI;
                                sweepTo = 180 * Math.Atan2(Math.Abs(cY - gH + arc.End.Y), Math.Abs(arc.End.X - cX)) / Math.PI;

                                sweepFrom = adjustAngle(sweepFrom, gH - arc.Start.Y - cY, arc.Start.X - cX);
                                sweepTo = adjustAngle(sweepTo, gH - arc.End.Y - cY, arc.End.X - cX);
                            }

                            //Deal with colour
                            var sv = c.Stroke as SvgColourServer;
                            if (cacheC1 != string.Format("c1 := RGB.AddColour({0}, {1}, {2})", sv.Colour.R / 255f, sv.Colour.G / 255f, sv.Colour.B / 255f))
                            {
                                cacheC1 = string.Format("c1 := RGB.AddColour({0}, {1}, {2})", sv.Colour.R / 255f, sv.Colour.G / 255f, sv.Colour.B / 255f);
                                sb.AppendLine(cacheC1);
                            }
                            if (sv.ToString() != "none")
                                sb.AppendLine(drawArc((int)cX, (int)cY, (int)arc.RadiusX, (int)arc.RadiusY, (int)sweepFrom, (int)sweepTo, getColor(sv.Colour.Name, false)));
                        }
                        #endregion

                        #region Cubic Beziers
                        // Bezier curves
                        // Full credits to: https://github.com/domoszlai/bezier2biarc
                        // for the implementation. You are awesome! I don't have to read those research papers now  :P
                        else if (p is SvgCubicCurveSegment)
                        {
                            var b = p as SvgCubicCurveSegment;

                            // Broken. TODO: Fix this please
                            sb.AppendLine("% Bezier curves currenly broken. Sorry. Element id [" + c.ID + "]");
                            continue; //Bezier is broken

                            CubicBezier curve = new CubicBezier(
                                new Vector2(b.Start.X, b.Start.Y),
                                new Vector2(b.FirstControlPoint.X, b.FirstControlPoint.Y),
                                new Vector2(b.SecondControlPoint.X, b.SecondControlPoint.Y),
                                new Vector2(b.End.X, b.End.Y));

                            /*curve = new CubicBezier(
                                new Vector2(100, 500), new Vector2(150, 100), new Vector2(500, 150), new Vector2(350, 350));*/

                            var biarcs = Algorithm.ApproxCubicBezier(curve, 10, 1);
                            foreach (var biarc in biarcs)
                            {
                                int r1 = (int)Math.Ceiling((biarc.A1.startAngle * 180.0f / (float)Math.PI) > 360 ? -(biarc.A1.startAngle * 180.0f / (float)Math.PI) : 360 - (biarc.A1.startAngle * 180.0f / (float)Math.PI));
                                int r2 = (int)Math.Ceiling(biarc.A1.sweepAngle * 180.0f / (float)Math.PI);
                                int r3 = (int)Math.Ceiling((biarc.A2.startAngle * 180.0f / (float)Math.PI) > 360 ? -(biarc.A2.startAngle * 180.0f / (float)Math.PI) : 360 - (biarc.A2.startAngle * 180.0f / (float)Math.PI));
                                int r4 = (int)Math.Ceiling(biarc.A2.sweepAngle * 180.0f / (float)Math.PI);

                                sb.AppendLine(string.Format("Draw.Arc({0}, {1}, {2}, {3}, {4}, {5}, red)",
                                    (int)(biarc.A1.C.X),
                                    (int)(gH - biarc.A1.C.Y),
                                    (int)(biarc.A1.r),
                                    (int)(biarc.A1.r),
                                    (int)(r1),
                                    (int)(r1 + r2)));

                                sb.AppendLine(string.Format("Draw.Arc({0}, {1}, {2}, {3}, {4}, {5}, red)",
                                    (int)(biarc.A2.C.X),
                                    (int)(gH - biarc.A2.C.Y),
                                    (int)(biarc.A2.r),
                                    (int)(biarc.A2.r),
                                    (int)(r3),
                                    (int)(r3 + r4)));
                            } //End foreach
                        } //End if
                        #endregion

                    } //End foreach
                } //End if
                #endregion

                #region Ellipse
                else if (c is SvgEllipse)
                {
                    var e = c as SvgEllipse;
                    var sColor = e.Stroke as SvgColourServer;
                    var fColor = e.Fill as SvgColourServer;

                    if ((sColor.ToString() != "none") && !isTuringColor(sColor.Colour.ToString()))
                    {
                        if (cacheC1 != string.Format("c1 := RGB.AddColour({0}, {1}, {2})", sColor.Colour.R / 255f, sColor.Colour.G / 255f, sColor.Colour.B / 255f))
                        {
                            cacheC1 = string.Format("c1 := RGB.AddColour({0}, {1}, {2})", sColor.Colour.R / 255f, sColor.Colour.G / 255f, sColor.Colour.B / 255f);
                            sb.AppendLine(cacheC1);
                        }
                    }

                    if ((fColor.ToString() != "none") && !isTuringColor(fColor.Colour.ToString()))
                    {
                        if (cacheC2 != string.Format("c2 := RGB.AddColour({0}, {1}, {2})", fColor.Colour.R / 255f, fColor.Colour.G / 255f, fColor.Colour.B / 255f))
                        {
                            cacheC2 = string.Format("c2 := RGB.AddColour({0}, {1}, {2})", fColor.Colour.R / 255f, fColor.Colour.G / 255f, fColor.Colour.B / 255f);
                            sb.AppendLine(cacheC2);
                        }
                    }

                    // Check and apply the stroke
                    if ((e.StrokeWidth.Value > 0) && (sColor.ToString() != "none") && (!sColor.Colour.IsEmpty) && (sColor.Colour.A > 0))
                    {
                        int width = (int)e.StrokeWidth.Value;
                        sb.AppendLine(drawFillOval((int)e.CenterX, gH - (int)e.CenterY, (int)e.RadiusX + width, (int)e.RadiusY + width, getColor(sColor.Colour.ToString(), false)));
                    }

                    // Check if it is filled
                    if ((fColor.ToString() != "none") && (!fColor.Colour.IsEmpty) && (fColor.Colour.A > 0))
                        sb.AppendLine(drawFillOval((int)e.CenterX.Value, gH - (int)e.CenterY.Value, (int)e.RadiusX.Value, (int)e.RadiusY.Value, getColor(fColor.Colour.Name, true)));
                    else
                        sb.AppendLine(drawOval((int)e.CenterX.Value, gH - (int)e.CenterY.Value, (int)e.RadiusX.Value, (int)e.RadiusY.Value, getColor(sColor.Colour.Name, false)));
                }
                #endregion

                #region Circles
                else if (c is SvgCircle)
                {
                    var e = c as SvgCircle;
                    var sColor = e.Stroke as SvgColourServer;
                    var fColor = e.Fill as SvgColourServer;

                    if ((sColor.ToString() != "none") && !isTuringColor(sColor.Colour.ToString()))
                    {
                        if (cacheC1 != string.Format("c1 := RGB.AddColour({0}, {1}, {2})", sColor.Colour.R / 255f, sColor.Colour.G / 255f, sColor.Colour.B / 255f))
                        {
                            cacheC1 = string.Format("c1 := RGB.AddColour({0}, {1}, {2})", sColor.Colour.R / 255f, sColor.Colour.G / 255f, sColor.Colour.B / 255f);
                            sb.AppendLine(cacheC1);
                        }
                    }

                    if ((fColor.ToString() != "none") && !isTuringColor(fColor.Colour.ToString()))
                    {
                        if (cacheC2 != string.Format("c2 := RGB.AddColour({0}, {1}, {2})", fColor.Colour.R / 255f, fColor.Colour.G / 255f, fColor.Colour.B / 255f))
                        {
                            cacheC2 = string.Format("c2 := RGB.AddColour({0}, {1}, {2})", fColor.Colour.R / 255f, fColor.Colour.G / 255f, fColor.Colour.B / 255f);
                            sb.AppendLine(cacheC2);
                        }
                    }

                    // Check and apply the stroke
                    if ((e.StrokeWidth.Value > 0) && (sColor.ToString() != "none") && (!sColor.Colour.IsEmpty) && (sColor.Colour.A > 0))
                    {
                        int width = (int)e.StrokeWidth.Value;
                        sb.AppendLine(drawFillOval((int)e.CenterX, gH - (int)e.CenterY, (int)e.Radius + width, (int)e.Radius + width, getColor(sColor.Colour.ToString(), false)));
                    }

                    //Check if it is filled
                    if ((fColor.ToString() != "none") && (!fColor.Colour.IsEmpty) && (fColor.Colour.A > 0))
                        sb.AppendLine(drawFillOval((int)e.CenterX.Value, gH - (int)e.CenterY.Value, (int)e.Radius.Value, (int)e.Radius.Value, getColor(fColor.Colour.Name, true)));
                    else
                        sb.AppendLine(drawOval((int)e.CenterX.Value, gH - (int)e.CenterY.Value, (int)e.Radius.Value, (int)e.Radius.Value, getColor(sColor.Colour.Name, false)));

                } //End if
                #endregion

            } // End foreach

            //Return
            return sb.ToString();
        }

        #endregion

        #region Turing Drawing Methods

        static string drawLine(int x1, int y1, int x2, int y2, int thick, string c)
        {
            if (thick == 1)
                return string.Format("drawline({0}, {1}, {2}, {3}, {4})", x1, gH - y1, x2, gH - y2, c);
            else
                return string.Format("Draw.ThickLine({0}, {1}, {2}, {3}, {4}, {5})", x1, gH - y1, x2, gH - y2, thick, c);
        }

        static string drawBox(int x1, int y1, int x2, int y2, string c)
        {
            return string.Format("drawbox({0}, {1}, {2}, {3}, {4}", x1, y1, x2, y2, c);
        }

        static string drawFillBox(int x1, int y1, int x2, int y2, string c)
        {
            return string.Format("drawfillbox({0}, {1}, {2}, {3}, {4}", x1, y1, x2, y2, c);
        }

        static string drawOval(int x, int y, int rx, int ry, string c)
        {
            return string.Format("drawoval({0}, {1}, {2}, {3}, {4})", x, y, rx, ry, c);
        }

        static string drawFillOval(int x, int y, int rx, int ry, string c)
        {
            return string.Format("drawfilloval({0}, {1}, {2}, {3}, {4})", x, y, rx, ry, c);
        }

        static string drawArc(int x, int y, int xRad, int yRad, int from, int to, string c)
        {
            return string.Format("drawarc({0}, {1}, {2}, {3}, {4}, {5}, {6})", x, y, xRad, yRad, from, to, c);
        }

        #endregion

        #region Conversion to Turing Space

        static double pCenterX(double x1, double y1, double x2, double y2, double radius)
        {
            double radsq = radius * radius;
            double q = Math.Sqrt(((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1)));
            double x3 = (x1 + x2) / 2;
            return x3 - Math.Sqrt(radsq - ((q / 2) * (q / 2))) * ((y1 - y2) / q);
        }

        static double pCenterY(double x1, double y1, double x2, double y2, double radius)
        {
            double radsq = radius * radius;
            double q = Math.Sqrt(((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1)));
            double y3 = (y1 + y2) / 2;
            return y3 - Math.Sqrt(radsq - ((q / 2) * (q / 2))) * ((x2 - x1) / q);
        }

        static double nCenterX(double x1, double y1, double x2, double y2, double radius)
        {
            double radsq = radius * radius;
            double q = Math.Sqrt(((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1)));
            double x3 = (x1 + x2) / 2;
            return x3 + Math.Sqrt(radsq - ((q / 2) * (q / 2))) * ((y1 - y2) / q);
        }

        static double nCenterY(double x1, double y1, double x2, double y2, double radius)
        {
            double radsq = radius * radius;
            double q = Math.Sqrt(((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1)));
            double y3 = (y1 + y2) / 2;
            return y3 + Math.Sqrt(radsq - ((q / 2) * (q / 2))) * ((x2 - x1) / q);
        }

        static double adjustAngle(double a, double y, double x)
        {
            if ((x > 0) && (y > 0)) return a;
            else if ((x < 0) && (y > 0)) return 180 - a;
            else if ((x < 0) && (y < 0)) return 180 + a;
            else return -a;
        }

        static bool isTuringColor(string color)
        {
            return turingColors.Contains(color);
        }

        static string getColor(string color, bool isFill)
        {
            return isTuringColor(color) ? color : isFill ? "c2" : "c1";
        }

        #endregion
    }
}
