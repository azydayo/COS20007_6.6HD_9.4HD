using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public static class ImageFitter
{
    public static Rectangle GetFittingRectangle(Texture2D image, int windowWidth, int windowHeight)
    {
        float imageAspect = (float)image.Width / image.Height;
        float windowAspect = (float)windowWidth / windowHeight;

        int drawWidth, drawHeight;

        if (imageAspect > windowAspect)
        {
            drawHeight = windowHeight;
            drawWidth = (int)(windowHeight * imageAspect);
        }
        else
        {
            drawWidth = windowWidth;
            drawHeight = (int)(windowWidth / imageAspect);
        }

        int x = (windowWidth - drawWidth) / 2;
        int y = (windowHeight - drawHeight) / 2;

        return new Rectangle(x, y, drawWidth, drawHeight);
    }

    public static Rectangle GetFillingRectangle(Texture2D image, int windowWidth, int windowHeight)
    {
        float imageAspect = (float)image.Width / image.Height;
        float windowAspect = (float)windowWidth / windowHeight;

        int drawWidth, drawHeight;

        if (imageAspect < windowAspect)
        {
            // Image is narrower than window: fill width, crop height
            drawWidth = windowWidth;
            drawHeight = (int)(windowWidth / imageAspect);
        }
        else
        {
            // Image is wider than window: fill height, crop width
            drawHeight = windowHeight;
            drawWidth = (int)(windowHeight * imageAspect);
        }

        int x = (windowWidth - drawWidth) / 2;
        int y = (windowHeight - drawHeight) / 2;

        return new Rectangle(x, y, drawWidth, drawHeight);
    }
}

