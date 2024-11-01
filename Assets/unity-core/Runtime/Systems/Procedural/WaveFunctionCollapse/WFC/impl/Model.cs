/*
The MIT License(MIT)
Copyright(c) mxgmn 2016.
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
The software is provided "as is", without warranty of any kind, express or implied, including but not limited to the warranties of merchantability, fitness for a particular purpose and noninfringement. In no event shall the authors or copyright holders be liable for any claim, damages or other liability, whether in an action of contract, tort or otherwise, arising from, out of or in connection with the software or the use or other dealings in the software.
*/

using System;

public abstract class Model
{
	protected bool[][] wave;

	protected int[][][] propagator;
	int[][][] compatible;
	protected int[] observed;

	protected bool init;

	Tuple<int, int>[] stack;
	int stacksize;

	protected Random random;
	protected int FMX, FMY, T;
	protected bool periodic;

	protected double[] weights;
	double[] weightLogWeights;

	int[] sumsOfOnes;
	double sumOfWeights, sumOfWeightLogWeights, startingEntropy;
	double[] sumsOfWeights, sumsOfWeightLogWeights, entropies;

	protected Model(int width, int height)
	{
		this.FMX = width;
		this.FMY = height;
	}

	void Init()
	{
		this.wave = new bool[this.FMX * this.FMY][];
		this.compatible = new int[this.wave.Length][][];
		for (var i = 0; i < this.wave.Length; i++)
		{
			this.wave[i] = new bool[this.T];
			this.compatible[i] = new int[this.T][];
			for (var t = 0; t < this.T; t++) this.compatible[i][t] = new int[4];
		}

		this.weightLogWeights = new double[this.T];
		this.sumOfWeights = 0;
		this.sumOfWeightLogWeights = 0;

		for (var t = 0; t < this.T; t++)
		{
			this.weightLogWeights[t] = this.weights[t] * Math.Log(this.weights[t]);
			this.sumOfWeights += this.weights[t];
			this.sumOfWeightLogWeights += this.weightLogWeights[t];
		}

		this.startingEntropy = Math.Log(this.sumOfWeights) - this.sumOfWeightLogWeights / this.sumOfWeights;

		this.sumsOfOnes = new int[this.FMX * this.FMY];
		this.sumsOfWeights = new double[this.FMX * this.FMY];
		this.sumsOfWeightLogWeights = new double[this.FMX * this.FMY];
		this.entropies = new double[this.FMX * this.FMY];

		this.stack = new Tuple<int, int>[this.wave.Length * this.T];
		this.stacksize = 0;
	}

	

	bool? Observe()
	{
		var min = 1E+3;
		var argmin = -1;

		for (var i = 0; i < this.wave.Length; i++)
		{
			if (this.OnBoundary(i % this.FMX, i / this.FMX)) continue;

			var amount = this.sumsOfOnes[i];
			if (amount == 0) return false;

			var entropy = this.entropies[i];
			if (amount > 1 && entropy <= min)
			{
				var noise = 1E-6 * this.random.NextDouble();
				if (entropy + noise < min)
				{
					min = entropy + noise;
					argmin = i;
				}
			}
		}

		if (argmin == -1)
		{
			this.observed = new int[this.FMX * this.FMY];
			for (var i = 0; i < this.wave.Length; i++) for (var t = 0; t < this.T; t++) if (this.wave[i][t]) {
				this.observed[i] = t; break; }
			return true;
		}

		var distribution = new double[this.T];
		for (var t = 0; t < this.T; t++) distribution[t] = this.wave[argmin][t] ? this.weights[t] : 0;
		var r = distribution.Random(this.random.NextDouble());
		
		var w = this.wave[argmin];
		for (var t = 0; t < this.T; t++)	if (w[t] != (t == r))
			this.Ban(argmin, t);

		return null;
	}

	protected void Propagate()
	{
		while (this.stacksize > 0)
		{
			var e1 = this.stack[this.stacksize - 1];
			this.stacksize--;

			var i1 = e1.Item1;
			int x1 = i1 % this.FMX, y1 = i1 / this.FMX;

			for (var d = 0; d < 4; d++)
			{
				int dx = DX[d], dy = DY[d];
				int x2 = x1 + dx, y2 = y1 + dy;
				if (this.OnBoundary(x2, y2)) continue;

				if (x2 < 0) x2 += this.FMX;
				else if (x2 >= this.FMX) x2 -= this.FMX;
				if (y2 < 0) y2 += this.FMY;
				else if (y2 >= this.FMY) y2 -= this.FMY;

				var i2 = x2 + y2 * this.FMX;
				var p = this.propagator[d][e1.Item2];
				var compat = this.compatible[i2];

				for (var l = 0; l < p.Length; l++)
				{
					var t2 = p[l];
					var comp = compat[t2];

					comp[d]--;
					if (comp[d] == 0) this.Ban(i2, t2);
				}
			}
		}
	}

	public bool Run(int seed, int limit)
	{
		if (this.wave == null) this.Init();

		if (!this.init) {
			this.init = true;
			this.Clear();
		}

		if (seed==0) {
			this.random = new Random();
		} else {
			this.random = new Random(seed);
		}

		for (var l = 0; l < limit || limit == 0; l++)
		{
			var result = this.Observe();
			if (result != null) return (bool)result;
			this.Propagate();
		}

		return true;
	}

	protected void Ban(int i, int t)
	{
		this.wave[i][t] = false;

		var comp = this.compatible[i][t];
		for (var d = 0; d < 4; d++) comp[d] = 0;
		this.stack[this.stacksize] = new Tuple<int, int>(i, t);
		this.stacksize++;

		var sum = this.sumsOfWeights[i];
		this.entropies[i] += this.sumsOfWeightLogWeights[i] / sum - Math.Log(sum);

		this.sumsOfOnes[i] -= 1;
		this.sumsOfWeights[i] -= this.weights[t];
		this.sumsOfWeightLogWeights[i] -= this.weightLogWeights[t];

		sum = this.sumsOfWeights[i];
		this.entropies[i] -= this.sumsOfWeightLogWeights[i] / sum - Math.Log(sum);
	}

	protected virtual void Clear()
	{
		for (var i = 0; i < this.wave.Length; i++)
		{
			for (var t = 0; t < this.T; t++)
			{
				this.wave[i][t] = true;
				for (var d = 0; d < 4; d++) this.compatible[i][t][d] = this.propagator[opposite[d]][t].Length;
			}

			this.sumsOfOnes[i] = this.weights.Length;
			this.sumsOfWeights[i] = this.sumOfWeights;
			this.sumsOfWeightLogWeights[i] = this.sumOfWeightLogWeights;
			this.entropies[i] = this.startingEntropy;
		}
	}

	protected abstract bool OnBoundary(int x, int y);

	protected static int[] DX = { -1, 0, 1, 0 };
	protected static int[] DY = { 0, 1, 0, -1 };
	static int[] opposite = { 2, 3, 0, 1 };
}