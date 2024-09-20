using UnityEngine;

namespace Core.Input
{
    public interface IInputHandler
    {
        bool Lock { get; set; }
        public Vector3 Direction { get; }
        FloatingJoystick Joystick { get; }
    }

    public class InputHandler : FloatingJoystick, IInputHandler
    {
        public bool Lock
        {
            get => !gameObject.activeSelf;
            set => gameObject.SetActive(!value);
        }

        public Vector3 Direction  => new Vector3(Horizontal, 0, Vertical);
        public FloatingJoystick Joystick => this;
    }
}