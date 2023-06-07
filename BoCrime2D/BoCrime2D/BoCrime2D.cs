using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

namespace BoCrime2D;

public class BoCrime2D : PhysicsGame
{
    private const double NOPEUS = 130;
    private const double HYPPYNOPEUS = 350;
    private const int RUUDUN_KOKO = 40;
    pelaajan1Ase.Position = pelaaja1.Position + new Vector(pelaaja1.Width / 2, 0);
   
    pelaajan1Ase = new AssaultRifle(30, 10);
    
    pelaajan1Ase.Ammo.Value = 1000; 


    pelaajan1Ase.FireRate = 5;


    pelaajan1Ase.ProjectileCollision = AmmusOsui;


    pelaajaniAse.Position = pelaaja1.Position;

    pelaaja1.Add(pelaajan1Ase);
    
    
    pelaaja1.Weapon = new AssaultRifle(30, 10);


    pelaaja1.Weapon.Ammo.Value = 1000;


    pelaaja1.Weapon.ProjectileCollision = AmmusOsui;
    
    //pelaaja1 on PlatformCharacter-tyyppinen
    pelaaja1.Weapon = new AssaultRifle(30, 10);

//Ammusten määrä aluksi:
    pelaaja1.Weapon.Ammo.Value = 1000;

//Mitä tapahtuu kun ammus osuu johonkin?
    pelaaja1.Weapon.ProjectileCollision = AmmusOsui;

    private PlatformCharacter pelaaja1;

    private Image pelaajanKuva = LoadImage("tero-removebg-preview.png");
    private Image tahtiKuva = LoadImage("massi.png");

    private SoundEffect maaliAani = LoadSoundEffect("maali.wav");
    
    AssaultRifle pelaajan1Ase;

    Image taustaKuva = LoadImage("boocity");
    

    public override void Begin()
    {
        Gravity = new Vector(0, -1000);

        LuoKentta();
        LisaaNappaimet();

        Camera.Follow(pelaaja1);
        Camera.ZoomFactor = 0.01 ;
        Camera.StayInLevel = true;

        MasterVolume = 0.5;
    }

    private void LuoKentta()
    {
        TileMap kentta = TileMap.FromLevelAsset("kentta1.txt");
        kentta.SetTileMethod('-', LisaaTaso);
        kentta.SetTileMethod('*', LisaaTahti);
        kentta.SetTileMethod('N', LisaaPelaaja);
        kentta.Execute(RUUDUN_KOKO, RUUDUN_KOKO);
        Level.Width = 4000;
        Level.Height = 250;
        Level.Background.Image = taustaKuva;
        Level.Background.ScaleToLevelByHeight();
        Level.Background.TileToLevel();
        Level.CreateBottomBorder();
        Level.CreateLeftBorder();
        Level.CreateRightBorder();
        Level.CreateTopBorder();


    }

    private void LisaaTaso(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject taso = PhysicsObject.CreateStaticObject(leveys, korkeus);
        taso.Position = paikka;
        taso.Color = Color.Red;
        Add(taso);
    }

    private void LisaaTahti(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject tahti = PhysicsObject.CreateStaticObject(leveys, korkeus);
        tahti.IgnoresCollisionResponse = true;
        tahti.Position = paikka;
        tahti.Image = tahtiKuva;
        tahti.Tag = "tahti";
        Add(tahti);
    }

    void AmmusOsui(PhysicsObject ammus, PhysicsObject kohde)
    {
        
    }
    
    void AmmuAseella(AssaultRifle ase)
    {
        PhysicsObject ammus = ase.Shoot();

        if (ammus != null)
        {
            void AmmusOsui(PhysicsObject ammus, PhysicsObject kohde)
            {
                //ammus.Destroy();
            }
        }
    }
    
    private void LisaaPelaaja(Vector paikka, double leveys, double korkeus)
    {
        pelaaja1 = new PlatformCharacter(leveys, korkeus);
        pelaaja1.Position = paikka;
        pelaaja1.Mass = 10.0;
        pelaaja1.Image = pelaajanKuva;
        AddCollisionHandler(pelaaja1, "tahti", TormaaTahteen);
        Add(pelaaja1);
    }

    private void LisaaNappaimet()
    {
        Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä ohjeet");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");

        Keyboard.Listen(Key.A, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja1, -NOPEUS);
        Keyboard.Listen(Key.D, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja1, NOPEUS);
        Keyboard.Listen(Key.W, ButtonState.Pressed, Hyppaa, "Pelaaja hyppää", pelaaja1, HYPPYNOPEUS);

        ControllerOne.Listen(Button.Back, ButtonState.Pressed, Exit, "Poistu pelistä");

        ControllerOne.Listen(Button.DPadLeft, ButtonState.Down, Liikuta, "Pelaaja liikkuu vasemmalle", pelaaja1,
            -NOPEUS);
        ControllerOne.Listen(Button.DPadRight, ButtonState.Down, Liikuta, "Pelaaja liikkuu oikealle", pelaaja1, NOPEUS);
        ControllerOne.Listen(Button.A, ButtonState.Pressed, Hyppaa, "Pelaaja hyppää", pelaaja1, HYPPYNOPEUS);
        Keyboard.Listen(Key.Space, ButtonState.Down, AmmuAseella, "Ammu", pelaajan1Ase);


        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
    }

    private void Liikuta(PlatformCharacter hahmo, double nopeus)
    {
        hahmo.Walk(nopeus);
    }

    private void Hyppaa(PlatformCharacter hahmo, double nopeus)
    {
        hahmo.Jump(nopeus);
    }

    private void TormaaTahteen(PhysicsObject hahmo, PhysicsObject tahti)
    {
        maaliAani.Play();
        MessageDisplay.Add("+1000$");
        tahti.Destroy();
    }
    
}