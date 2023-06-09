using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Net.Sockets;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;
using SixLabors.ImageSharp;
using Color = Jypeli.Color;
using Image = Jypeli.Image;

namespace BoCrime2D;

public class BoCrime2D : PhysicsGame
{
    private const double NOPEUS = 130;
    private const double HYPPYNOPEUS = 500;
    private const int RUUDUN_KOKO = 40;


    
    

    private PlatformCharacter pelaaja1;

    private Image pelaajanKuva = LoadImage("tero-removebg-preview.png");
    private Image tahtiKuva = LoadImage("Ctero-removebg-preview.png");
    
    
    AssaultRifle pelaajan1Ase;

    Image taustaKuva = LoadImage("boocity");
    
    SoundEffect kuolema = LoadSoundEffect("gta-v-death-sound-effect-102");
    
    DoubleMeter alaspainlaskuri;
    Timer aikalaskuri;
    private Label aikanaytto;
    
    public override void Begin()
    {
       

        alkuvalikko();
       
        MasterVolume = 0.5;
    }

    private void alkuvalikko()
    {
        MultiSelectWindow alkuvalikko = new MultiSelectWindow("BO Crime 2D", "Start Game", "Quit");

        alkuvalikko.AddItemHandler(0, AloitaPeli);
        
        alkuvalikko.AddItemHandler(1, Exit);

        alkuvalikko.Color = Color.DarkGray;
        alkuvalikko.SetButtonColor(Color.Black);
        alkuvalikko.SetButtonTextColor(Color.White);
        PushButton[] nappulat = alkuvalikko.Buttons;
        Level.Background.Image = LoadImage("gtaa");
        Level.Width = 10;
        Level.Height = 1020;
        Level.Background.ScaleToLevelByWidth();
        Level.Background.ScaleToLevelByHeight();



        alkuvalikko.DefaultCancel = 3;

        Add(alkuvalikko);
        
    }

    private void AloitaPeli()
    {
        LuoKentta();
        LisaaNappaimet();
        Mouse.MouseCursor = MouseCursor.Crosshair;
        LuoAikalaskuri();
        LaskeAlaspain();
        aikalaskuri.Start();
    }
    private void LuoKentta()
    {
        TileMap kentta = TileMap.FromLevelAsset("kentta1.txt");
        kentta.SetTileMethod('-', LisaaTaso);
        kentta.SetTileMethod('*', LisaaTahti);
        kentta.SetTileMethod('N', LisaaPelaaja);
        kentta.Execute(RUUDUN_KOKO, RUUDUN_KOKO);
        Level.Width = 4950;
        Level.Height = 250;
        Level.Background.Image = taustaKuva;
        Level.Background.ScaleToLevelByHeight();
        Level.Background.TileToLevel();
        Level.CreateBottomBorder();
        Level.CreateTopBorder();
        Level.CreateLeftBorder();
        Level.CreateRightBorder();
        Camera.Follow(pelaaja1);
        Camera.ZoomFactor = 0.01;
        Camera.StayInLevel = true;
        Gravity = new Vector(0, -1000);


    }
    
    

    private void LisaaTaso(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject taso = PhysicsObject.CreateStaticObject(leveys, korkeus);
        taso.Position = paikka;
        taso.Color = Color.DarkGray;
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
        if (kohde.Tag == "tahti")
        {
            kohde.Destroy();
            MessageDisplay.Add("Enemy killed!");
        }
       ammus.Destroy();
    }
    
    void AmmuAseella(AssaultRifle ase)
    {
        PhysicsObject ammus = ase.Shoot();
        
       
        if (ammus != null)
        { ammus.Size = new Vector(5, 2);
            ammus.MaximumLifetime = new TimeSpan(0,0,0,0,200);
        }
    }
    
    void Tahtaa()
    {
        Vector suunta = (Mouse.PositionOnWorld - pelaaja1.Weapon.AbsolutePosition).Normalize();
        pelaaja1.Weapon.Angle = suunta.Angle;
    }
    
    private void LisaaPelaaja(Vector paikka, double leveys, double korkeus)
    {
        pelaaja1 = new PlatformCharacter(leveys, korkeus);
        pelaaja1.Position = paikka;
        pelaaja1.Mass = 10.0;
        pelaaja1.Image = pelaajanKuva;
        AddCollisionHandler(pelaaja1, "tahti", TormaaTahteen);
        Add(pelaaja1);

        pelaajan1Ase = new AssaultRifle(30, 5);
        pelaajan1Ase.Ammo.Value = 999999999;
        pelaajan1Ase.FireRate = 5;
        pelaaja1.Weapon = pelaajan1Ase;
        pelaaja1.Weapon.ProjectileCollision = AmmusOsui;
        pelaaja1.Weapon.Position = pelaaja1.Position + new Vector( 2, 6.5);
        pelaajan1Ase.Power.DefaultValue = 200;
        pelaaja1.Weapon.Color = Color.Transparent;
        pelaajan1Ase.Image = null;


    }
    
    void LuoAikalaskuri()
    {
        alaspainlaskuri = new DoubleMeter(40);
    
        aikalaskuri = new Timer();
        aikalaskuri.Interval = 0.1;
        aikalaskuri.Timeout += LaskeAlaspain;

        aikanaytto = new Label();
        aikanaytto.TextColor = Color.White;
        aikanaytto.DecimalPlaces = 1;
        aikanaytto.BindTo(alaspainlaskuri);
        aikanaytto.X = Screen.Right - 100;
        aikanaytto.Y = Screen.Top - 100;
        Add(aikanaytto);
    }

    void LaskeAlaspain()
    {
        alaspainlaskuri.Value -= 0.1;

        if (alaspainlaskuri.Value <= 0)
        {
            aikalaskuri.Stop();
            pelaaja1.Destroy();
            Label tekstikentta = new Label(2000.0, 250.0, "MISSION FAILED");
            tekstikentta.Color = Color.Black;
            tekstikentta.TextColor = Color.Red;
            Add(tekstikentta);
            
        }
    }
    

    private void LisaaNappaimet()
    {
        Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä ohjeet");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
        
        Mouse.ListenMovement(0.1, Tahtaa, "Tähtää aseella");

        Keyboard.Listen(Key.A, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja1, -NOPEUS);
        Keyboard.Listen(Key.D, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja1, NOPEUS);
        Keyboard.Listen(Key.Space, ButtonState.Pressed, Hyppaa, "Pelaaja hyppää", pelaaja1, HYPPYNOPEUS);

        ControllerOne.Listen(Button.Back, ButtonState.Pressed, Exit, "Poistu pelistä");

        ControllerOne.Listen(Button.DPadLeft, ButtonState.Down, Liikuta, "Pelaaja liikkuu vasemmalle", pelaaja1,
            -NOPEUS);
        ControllerOne.Listen(Button.DPadRight, ButtonState.Down, Liikuta, "Pelaaja liikkuu oikealle", pelaaja1, NOPEUS);
        ControllerOne.Listen(Button.A, ButtonState.Pressed, Hyppaa, "Pelaaja hyppää", pelaaja1, HYPPYNOPEUS);
        Mouse.Listen(MouseButton.Left, ButtonState.Down, AmmuAseella, "Ammu", pelaajan1Ase);


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
        kuolema.Play();
        pelaaja1.Destroy();
        aikanaytto.Destroy();
        
        Label tekstikentta = new Label(2000.0, 250.0, "YOU DIED");
        tekstikentta.Color = Color.Black;
        tekstikentta.TextColor = Color.Red;
        tekstikentta.Font.Size = 120;
        Add(tekstikentta);
    }
    
}