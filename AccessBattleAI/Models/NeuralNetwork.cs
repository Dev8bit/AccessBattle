﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Some of this code is based on the example code from a Microsoft keynote.
// Here is an article based on it.
// https://visualstudiomagazine.com/articles/2013/09/01/neural-network-training-using-back-propagation.aspx
// I'm not using Back-Propagation here.

namespace AccessBattleAI.Models
{
    public class NeuralNetwork
    {
        Random _rnd;

        int _numInput;
        int _numHidden;
        int _numOutput;

        double[] _inputs;
        public double[] Inputs => _inputs;

        double[][] _ihWeights; // Hidden node input weights
        double[] _hBiases; // Hidden node offsets
        double[][] _hoWeights; // Hidden node output weights

        double[] _hOutputs;
        double[] _oBiases; // Output node offsets

        double[] _outputs;
        public double[] Outputs => _outputs;

        private NeuralNetwork()
        {
            _rnd = new Random();
        }

        static int SeedBase = Environment.TickCount;
        static readonly object SeedLock = new object();

        public NeuralNetwork(int numInput, int numHidden, int numOutput, int? seed = null)
        {
            if (seed != null)
                _rnd = new Random(seed.Value); // For unit testing
            else
            {
                // Default constructor of Random uses Environment.TickCount
                // This approach tries to counter problems that might occur when multiple instances
                // are created within the same time frame.
                lock (SeedLock) { _rnd = new Random((++SeedBase).GetHashCode() ^ Environment.TickCount); }
            }

            _numInput = numInput;
            _numHidden = numHidden;
            _numOutput = numOutput;

            _inputs = new double[numInput];

            _ihWeights = MakeMatrix(numInput, numHidden);
            _hBiases = new double[numHidden];
            _hOutputs = new double[numHidden];

            _hoWeights = MakeMatrix(numHidden, numOutput);
            _oBiases = new double[numOutput];

            _outputs = new double[numOutput];
        }

        static double[][] MakeMatrix(int rows, int cols) // helper for ctor
        {
            double[][] result = new double[rows][];
            for (int r = 0; r < result.Length; ++r)
                result[r] = new double[cols];
            return result;
        }

        public void Mutate(double delta)
        {
            // Change the weights and biases slightly
            for (int ni = 0; ni < _numInput; ++ni)
            {
                for (int nh = 0; nh < _numHidden; ++nh)
                {
                    _ihWeights[ni][nh] += RandomBetween(delta, -delta);
                }
            }
            for (int nh = 0; nh < _numHidden; ++nh)
            {
                _hBiases[nh] += RandomBetween(delta, -delta);
            }
            for (int no = 0; no < _numOutput; ++no)
            {
                _oBiases[no] += RandomBetween(delta, -delta);
            }
            for (int nh = 0; nh < _numHidden; ++nh)
            {
                for (int no = 0; no < _numOutput; ++no)
                {
                    _hoWeights[nh][no] += RandomBetween(delta, -delta);
                }
            }

        }

        double RandomBetween(double min, double max) => _rnd.NextDouble() * (max - min) + min;

        public void ComputeOutputs()
        {
            double[] hSums = new double[_numHidden]; // hidden nodes sums scratch array
            double[] oSums = new double[_numOutput]; // output nodes sums

            for (int j = 0; j < _numHidden; ++j)  // compute i-h sum of weights * inputs
                for (int i = 0; i < _numInput; ++i)
                    hSums[j] += _inputs[i] * _ihWeights[i][j]; // note +=

            for (int i = 0; i < _numHidden; ++i)  // add biases to input-to-hidden sums
                hSums[i] += _hBiases[i];

            for (int i = 0; i < _numHidden; ++i)   // apply activation
                _hOutputs[i] = HyperTanFunction(hSums[i]); // hard-coded

            for (int j = 0; j < _numOutput; ++j)   // compute h-o sum of weights * hOutputs
                for (int i = 0; i < _numHidden; ++i)
                    oSums[j] += _hOutputs[i] * _hoWeights[i][j];

            for (int i = 0; i < _numOutput; ++i)  // add biases to input-to-hidden sums
                oSums[i] += _oBiases[i];

            double[] softOut = Softmax(oSums); // softmax activation does all outputs at once for efficiency
            Array.Copy(softOut, _outputs, softOut.Length);
        } // ComputeOutputs

        static double HyperTanFunction(double x)
        {
            if (x < -20.0) return -1.0; // approximation is correct to 30 decimals
            else if (x > 20.0) return 1.0;
            else return Math.Tanh(x);
        }

        static double[] Softmax(double[] oSums) // does all output nodes at once so scale doesn't have to be re-computed each time
        {
            // determine max output sum
            double max = oSums[0];
            for (int i = 0; i < oSums.Length; ++i)
                if (oSums[i] > max) max = oSums[i];

            // determine scaling factor -- sum of exp(each val - max)
            double scale = 0.0;
            for (int i = 0; i < oSums.Length; ++i)
                scale += Math.Exp(oSums[i] - max);

            double[] result = new double[oSums.Length];
            for (int i = 0; i < oSums.Length; ++i)
                result[i] = Math.Exp(oSums[i] - max) / scale;

            return result; // now scaled so that xi sum to 1.0
        }

        public string SaveAsString()
        {
            var sb = new StringBuilder();
            var info = System.Globalization.CultureInfo.InvariantCulture;

            sb.AppendLine("# Neural Network Definition");
            sb.AppendLine("# This file was automatically generated. Do not change!");
            sb.AppendLine();
            sb.AppendLine("# Number of nodes:");
            sb.AppendLine("Inputs: " + _numInput));
            sb.AppendLine("Hidden: "+ _numHidden);
            sb.AppendLine("Outputs: "+ _numOutput);
            sb.AppendLine();
            sb.AppendLine("# Biases:");
            sb.Append("BHidden: ");
            for (int i = 0; i < _hBiases.Length; ++i)
            {
                if (i != 0) sb.Append(";");
                sb.Append(_hBiases[i].ToString(info));
            }
            sb.AppendLine();
            sb.Append("BOutput: ");
            for (int i = 0; i < _oBiases.Length; ++i)
            {
                if (i != 0) sb.Append(";");
                sb.Append(_oBiases[i].ToString(info));
            }
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("# Weights:");
            sb.Append("WHidden: ");
            for (int i = 0; i < _ihWeights.Length; ++i)
            {
                for (int j = 0; j < _ihWeights[0].Length; ++j)
                {
                    if (i != 0 || j != 0) sb.Append(";");
                    sb.Append(_ihWeights[i][j].ToString(info));
                }
            }
            sb.AppendLine();
            sb.Append("WOutput: ");
            for (int i = 0; i < _hoWeights.Length; ++i)
            {
                for (int j = 0; j < _hoWeights[0].Length; ++j)
                {
                    if (i != 0 || j != 0) sb.Append(";");
                    sb.Append(_hoWeights[i][j].ToString(info));
                }
            }

            // The seed is not saved

            sb.AppendLine();
            return sb.ToString();
        }

        public bool SaveAsFile(string filename)
        {
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(filename))
                {
                    var info = System.Globalization.CultureInfo.InvariantCulture;

                    file.Write(SaveAsString());

                    file.Flush();
                    file.Close();
                }
            }
            catch (Exception e)
            {
                AccessBattle.Log.WriteLine(AccessBattle.LogPriority.Error, "Cannot save neural network. " + e.Message);
                return false;
            }
            return true;
        }

        public static NeuralNetwork ReadFromString(string data)
        {
            var network = new NeuralNetwork();
            using (StringReader reader = new StringReader(data))
            {
                try
                {
                    var info = System.Globalization.CultureInfo.InvariantCulture;
                    string line;
                    while ((line = reader.ReadLine()?.Trim()) != null)
                    {
                        if (line.StartsWith("#") || string.IsNullOrEmpty(line)) continue;

                        if (line.StartsWith("Inputs:"))
                        {
                            network._numInput = Convert.ToInt32(line.Substring("Inputs:".Length));
                        }
                        else if (line.StartsWith("Hidden:"))
                        {
                            network._numHidden = Convert.ToInt32(line.Substring("Hidden:".Length));
                        }
                        else if (line.StartsWith("Outputs:"))
                        {
                            network._numOutput = Convert.ToInt32(line.Substring("Outputs:".Length));
                        }
                        else if (line.StartsWith("BHidden:"))
                        {
                            int i = 0;
                            network._hBiases = new double[network._numHidden];
                            foreach (var str in line.Substring("BHidden:".Length).Split(';'))
                            {
                                network._hBiases[i++] = Convert.ToDouble(str, info);
                            }
                        }
                        else if (line.StartsWith("BOutput:"))
                        {
                            int i = 0;
                            network._oBiases = new double[network._numOutput];
                            foreach (var str in line.Substring("BOutput:".Length).Split(';'))
                            {
                                network._oBiases[i++] = Convert.ToDouble(str, info);
                            }
                        }
                        else if (line.StartsWith("WHidden:"))
                        {
                            network._ihWeights = MakeMatrix(network._numInput, network._numHidden);
                            int ni = 0;
                            int nh = 0;

                            foreach (var str in line.Substring("BOutput:".Length).Split(';'))
                            {
                                network._ihWeights[ni][nh] = Convert.ToDouble(str, info);
                                if (++nh >= network._numHidden)
                                {
                                    nh = 0;
                                    ++ni;
                                }
                            }
                        }
                        else if (line.StartsWith("WOutput:"))
                        {
                            network._hoWeights = MakeMatrix(network._numHidden, network._numOutput);

                            int nh = 0;
                            int no = 0;

                            foreach (var str in line.Substring("BOutput:".Length).Split(';'))
                            {
                                network._hoWeights[nh][no] = Convert.ToDouble(str, info);
                                if (++no >= network._numOutput)
                                {
                                    no = 0;
                                    ++nh;
                                }
                            }
                        }

                    }
                    // We do not do any further validations here. Software will crash soon enough if something was wrong.

                    // Init missing fields
                    network._inputs = new double[network._numInput];
                    network._outputs = new double[network._numOutput];
                    network._hOutputs = new double[network._numHidden];
                }
                catch (Exception e)
                {
                    AccessBattle.Log.WriteLine(AccessBattle.LogPriority.Error, "Cannot read neural network. " + e.Message);
                    return null;
                }
            }
            return network;
        }

        public static NeuralNetwork ReadFromFile(string filename)
        {
            try
            {
                return ReadFromString(System.IO.File.ReadAllText(filename));
            }
            catch (Exception e)
            {
                AccessBattle.Log.WriteLine(AccessBattle.LogPriority.Error, "Cannot read neural network. " + e.Message);
                return null;
            }
        }
    }
}
