using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlackScript : BlackWhiteScript
{
	protected override string Name
	{
		get
		{
			return "Black";
		}
	}

	private new void Start()
	{
		Start();
		for (int i = 0; i < _buttons.Length; i++)
		{
			int j = i;
			KMSelectable kmselectable = _buttons[j];
			kmselectable.OnInteract = (KMSelectable.OnInteractHandler)Delegate.Combine(kmselectable.OnInteract, new KMSelectable.OnInteractHandler(delegate()
			{
				Press(j);
				return false;
			}));
		}
	}

	private void Press(int j)
	{
		if (Errored)
		{
			return;
		}
		Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, _buttons[j].transform);
		_buttons[j].AddInteractionPunch(0.1f);
		if (!IsActive)
		{
			return;
		}
		_requiredPresses[j]--;
		if (_requiredPresses[j] < 0)
		{
			Log("You pressed button {0} one too many times. Strike!", new object[]
			{
				j + 1
			});
			Module.HandleStrike();
			IsActive = false;
			LightsOff();
			Module.HandlePass();
			Partner.Module.HandlePass();
			return;
		}
		if (_requiredPresses.All((int i) => i == 0))
		{
			Log("All buttons pressed correctly. Disarmed, for now...", new object[0]);
			IsActive = false;
			LightsOff();
			Module.HandlePass();
			Partner.Module.HandlePass();
		}
	}

	protected override void NeedyEnd()
	{
		Log("Time ran out. You needed to press the buttons this many more times: {0}", new object[]
		{
			_requiredPresses.Join(" ")
		});
		Module.HandleStrike();
		IsActive = false;
		LightsOff();
		Module.HandlePass();
		Partner.Module.HandlePass();
	}

	protected override void NeedyStart()
	{
		if (Partner == null)
		{
			Module.HandlePass();
			return;
		}
		IsActive = true;
		WhiteScript whiteScript = (WhiteScript)Partner;
		List<bool> list = (from i in Enumerable.Repeat<int>(0, 8)
		select UnityEngine.Random.Range(0, 2) == 1).ToList<bool>();
		list.Insert(UnityEngine.Random.Range(0, 8), true);
		for (int l = 0; l < 9; l++)
		{
			if (list[l])
			{
				whiteScript.Lights[l].On();
			}
			else
			{
				whiteScript.Lights[l].Off();
			}
		}
		for (int j = 0; j < _quadrants.Length; j++)
		{
			_requiredPresses[j] = 0;
			foreach (int index in _quadrants[j])
			{
				if (list[index])
				{
					_requiredPresses[j]++;
				}
			}
		}
		BWService.ActivateNeedy(Partner.NeedyComponent);
		string message = "Module activated! Linked white displayed {0}. Correct press counts are {1}.";
		object[] array2 = new object[2];
		array2[0] = (from b in list
		select (!b) ? "-" : "o").Join(string.Empty);
		array2[1] = _requiredPresses.Join(" ");
		Log(message, array2);
		Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.NeedyActivated, transform);
	}

	private void Update()
	{
		if (!IsActive || Errored)
		{
			return;
		}
		Partner.Module.SetNeedyTimeRemaining(Module.GetNeedyTimeRemaining());
	}

	private void LightsOff()
	{
		if (Errored)
		{
			return;
		}
		WhiteScript whiteScript = (WhiteScript)Partner;
		foreach (LightScript lightScript in whiteScript.Lights)
		{
			lightScript.Off();
		}
	}

	protected override void Activate()
	{
		if (Partner != null)
		{
			return;
		}
		WhiteScript[] componentsInChildren = transform.root.GetComponentsInChildren<WhiteScript>();
		WhiteScript whiteScript = componentsInChildren.FirstOrDefault((WhiteScript ws) => ws.Partner == null);
		if (whiteScript == null)
		{
			Error();
			Log("Not enough Whites spawned!", new object[0]);
			return;
		}
		Partner = whiteScript;
		whiteScript.Partner = this;
		if (componentsInChildren.Length > 1)
		{
			SetIdentifier(++Identifier);
			Partner.SetIdentifier(Identifier);
		}
	}

	protected override void BombOver()
	{
		IsActive = false;
		LightsOff();
	}

	private IEnumerator ProcessTwitchCommand(string command)
	{
		if (Errored)
		{
			yield break;
		}
		command = command.Trim();
		if (command.All((char c) => validChars.Contains(c) || char.IsWhiteSpace(c)))
		{
			if (command.Any((char c) => validChars.Contains(c)))
			{
				yield return null;
				foreach (char c2 in command)
				{
					switch (c2)
					{
					case '1':
					case '2':
					case '3':
					case '4':
						_buttons[int.Parse(c2.ToString()) - 1].OnInteract.Invoke();
						yield return new WaitForSeconds(0.1f);
						break;
					}
				}
			}
		}
		yield break;
	}

	private void TwitchHandleForcedSolve()
	{
		if (Errored)
		{
			return;
		}
		StartCoroutine(AutoSolve());
	}

	private IEnumerator AutoSolve()
	{
		IEnumerable<int> buttons = Enumerable.Range(0, 4);
		for (;;)
		{
			while (IsActive)
			{
				_buttons[buttons.First((int i) => _requiredPresses[i] != 0)].OnInteract.Invoke();
				yield return new WaitForSeconds(0.1f);
			}
			yield return null;
		}
		yield break;
	}

	[SerializeField]
	private KMSelectable[] _buttons;

	public bool IsActive;

	private int[][] _quadrants = new int[][]
	{
		new int[]
		{
			0,
			1,
			3,
			4
		},
		new int[]
		{
			1,
			2,
			4,
			5
		},
		new int[]
		{
			3,
			4,
			6,
			7
		},
		new int[]
		{
			4,
			5,
			7,
			8
		}
	};

	private int[] _requiredPresses = new int[4];

    private readonly string TwitchHelpMessage = "Use \"!{0} 1 123 34\" to press those buttons. Buttons are numbered in reading order.";

    private static readonly string validChars = "1234";
}