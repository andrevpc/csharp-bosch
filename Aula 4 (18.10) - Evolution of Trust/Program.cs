﻿// //Jogo
// Edjalma edjalma = new Edjalma();
// Travis travis = new Travis();
// Sword sword = new Sword();
// travis.Weapon = sword;
// travis.Attack(edjalma);
// Console.WriteLine(edjalma.Life);

// public abstract class Entity
// {
//     public string Name {get; protected set;} //Dano da arma não pode ser altarada fora
//     public int Life {get;protected set;}
//     public int Damage {get; protected set;}
//     public Weapon Weapon {get;set;}
    
//     public abstract void Attack(Entity target);
//     public abstract void ReceiveDamage(int damage);
// }

// public abstract class Weapon
// {
//     public string Name {get; protected set;}
//     public int Damage {get; protected set;}
// }

// public class Edjalma : Entity
// {

//     public int Shield { get; private set; }

//     public Edjalma()
//     {
//         this.Name = "Edjalma";
//         this.Life = 180;
//         this.Damage = 10;
//         this.Shield = 40;
//     }

//     public override void Attack(Entity target)
//     {
//         int damage = this.Damage / 2
//             + this.Weapon.Damage * 2;
//         target.ReceiveDamage(damage);
//     }
//     public override void ReceiveDamage(int damage)
//     {
//         if(this.Shield > damage)
//         {
//             this.Shield = 0;
//             return;
//         }
//         else
//         {
//             damage -= this.Shield;
//             this.Shield = 0;
//         }
//         if (damage < 5)
//             return;
//         this.Life -= damage;
//     }
// }

// public class Travis : Entity
// {
//     public Travis()
//     {
//         this.Name = "Travis";
//         this.Life = 20;
//         this.Damage = 80;
//     }

//     public override void Attack(Entity target)
//     {
//         int damage = 2*(Weapon?.Damage?? 0)
//             +2*this.Damage
//             + this.Weapon.Damage * 2;
//         target.ReceiveDamage(damage);
//     }
//     public override void ReceiveDamage(int damage)
//     {
//         if (damage < 5)
//             return;
//         this.Life -= damage;
//     }
// }

// public class Sword : Weapon
// {
//     public Sword()
//     {
//         this.Name = "Espada";
//         this.Damage = 5;
//     }
// }

// E, OU, NÃO, entradas

Input a = new Input();
Input b = new Input();
Input c = new Input();
AndGate and = new AndGate();
OrGate or = new OrGate();
NotGate not = new NotGate();

a.Connect(and);
b.Connect(and);
c.Connect(or);
and.Connect(or);
or.Connect(not);

b.IsOn = true;
c.IsOn = true;

Console.WriteLine(not.Output);

public abstract class Component
{
    public abstract bool Output { get; }

    public abstract void Update();

    protected Component Next { get; private set; }
    public void Connect(Component component)
    {
        this.Next = component;
        this.Next.ConnectInput(this);
    }

    protected virtual void ConnectInput(Component component) { }
}

public class Input : Component
{
    private bool ison = false;
    public bool IsOn
    {
        get => ison;
        set
        {
            this.ison = value;
            this.Update();
        }
    }

    public override bool Output => ison;

    public override void Update()
        => this.Next?.Update();
}

public abstract class BinaryComponent : Component
{    
    private Component inputA = null;
    private Component inputB = null;
    private bool state = false;

    public BinaryComponent(bool initial = false)
        => this.state = initial;

    public override bool Output => state;

    public override void Update()
    {
        var ia = inputA?.Output ?? false;
        var ib = inputB?.Output ?? false;
        var newValue = computeNew(ia, ib);

        if (newValue == state)
            return;
        
        state = newValue;
        this.Next.Update();
    }

    protected abstract bool computeNew(bool ia, bool ib);

    protected override void ConnectInput(Component component)
    {
        if (inputA == null)
            this.inputA = component;
        else if (inputB == null)
            this.inputB = component;
    }
}

public class AndGate : BinaryComponent
{
    protected override bool computeNew(bool ia, bool ib)
        => ia && ib;
}

public class NandGate : BinaryComponent
{
    public NandGate() : base(true) { }

    protected override bool computeNew(bool ia, bool ib)
        => !(ia && ib);
}

public class OrGate : BinaryComponent
{
    protected override bool computeNew(bool ia, bool ib)
        => ia || ib;
}

public class XorGate : BinaryComponent
{
    protected override bool computeNew(bool ia, bool ib)
        => ia ^ ib;
}

public class NotGate : Component
{
    private Component input = null;
    private bool state = true;

    public override bool Output => state;

    public override void Update()
    {
        var i = input?.Output ?? false;
        var newValue = !i;

        if (newValue == state)
            return;
        
        state = newValue;
        this.Next.Update();
    }

    protected override void ConnectInput(Component component)
    {
        if (input == null)
            this.input = component;
    }
}