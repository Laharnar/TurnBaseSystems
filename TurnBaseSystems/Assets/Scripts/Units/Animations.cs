public static class Animations {
    public static void SetAnimBool(this Unit t, bool value) {
        if (t && t.anim) {
            t.anim.SetBool("Walking", value);
        }
    }
}