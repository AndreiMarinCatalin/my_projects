using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace keymanager_Dev.Clases
{
    

    public sealed class KeyTimer
    {
        private static readonly KeyTimer _instance = new KeyTimer();
        private Timer _timer;
        private bool _booleanValue;
        public double TimeToChange { get; set; } = 10000;

        // Propiedad para acceder al singleton.
        public static KeyTimer Instance => _instance;

        public bool BooleanValue
        {
            get => _booleanValue;
            set
            {
                _booleanValue = value;
                ResetTimer();  // Reinicia el temporizador cada vez que cambiamos el valor booleano.
            }
        }

        

        // El constructor privado asegura que esta clase sólo pueda ser instanciada desde dentro.
        private KeyTimer()
        {
            _booleanValue = false;  // Establece el valor predeterminado en false.

            _timer = new Timer(TimeToChange);
            _timer.Elapsed += (sender, args) => BooleanValue = !BooleanValue;  // Cambia el valor booleano cuando se desencadena el temporizador.
            _timer.AutoReset = false;  // Para que el temporizador se ejecute sólo una vez.
        }

        private void ResetTimer()
        {
            if (_timer.Enabled)
            {
                _timer.Stop();  // Si el temporizador ya está en marcha, lo detenemos.
            }

            _timer.Interval = TimeToChange;
            _timer.Start();
        }
    }

}
