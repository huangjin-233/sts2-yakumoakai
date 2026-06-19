using System;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Combat;
using YakumoAkai.character.power;

public partial class Mps : Control
{
	private Creature _creature;
	private Label _mpLabel;
	private Control _mpcolor;
	private float _expectedMaxFgWidth = -1f;
	private Control _Mpcontorl;
	private Control _mp;

	public override void _Ready()
	{
		_mpLabel = GetNode<Label>("%mpshows");
		_mpcolor = GetNode<Control>("%mpcolor");
		_Mpcontorl = GetNode<Control>("%Mpcontorl");
		_mp = GetNode<Control>("%mp");
		NCreature? nCreature = GetParent().GetParent() as NCreature;
		_creature = nCreature.Entity;
	}
	private float MaxFgWidth
	{
		get
		{
			if (!(_expectedMaxFgWidth > 0f))
			{
				return _Mpcontorl.Size.X;
			}
			return _expectedMaxFgWidth;
		}
	}
	private float GetFgWidth(int amount)
	{
		return GetFgWidth(amount, MaxFgWidth);
	}
	private float GetFgWidth(int amount, float maxFgWidth)
	{
		if ( _creature.GetPowerAmount<mp>() <= 0)
		{
			return 0f;
		}
		float val = (float)amount / (float) mp.max * maxFgWidth;
		return Math.Max(val, ( _creature.GetPowerAmount<mp>() > 0) ? 12f : 0f);
	}
	public void RefreshValues()
	{
		RefreshText();
	}

	public override void _EnterTree()
	{
		CombatManager.Instance.StateTracker.CombatStateChanged += OnCombatStateChanged;
	}

	public override void _ExitTree()
	{
		CombatManager.Instance.StateTracker.CombatStateChanged -= OnCombatStateChanged;
	}
	private void OnCombatStateChanged(CombatState _)
	{
		if (_creature.GetPowerAmount<mp>() <= 0)
		{
			_mp.Visible = false;
			return;
		}
		_mp.Visible = true;
		RefreshValues();
		RefreshForeground();
	}
	private void RefreshText()
	{
		_mpLabel.Visible = true;
		_mpLabel.Text = _creature.GetPowerAmount<mp>().ToString()+"/" + mp.max.ToString();
	}

	private void RefreshForeground()
	{
		_mpcolor.Visible = true;
		_mpcolor.OffsetRight = GetFgWidth(_creature.GetPowerAmount<mp>())+ 10f;
	}
}
