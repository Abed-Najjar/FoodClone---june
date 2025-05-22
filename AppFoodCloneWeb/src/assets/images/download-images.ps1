# PowerShell script to download sample food and restaurant images
$imageUrls = @(
    # Hero banner images
    @{url="https://images.unsplash.com/photo-1504674900247-0877df9cc836"; name="hero-banner.jpg"},
    @{url="https://images.unsplash.com/photo-1555396273-367ea4eb4db5"; name="hero-banner2.jpg"},
    # Restaurant cover images
    @{url="https://images.unsplash.com/photo-1517248135467-4c7edcad34c4"; name="restaurant-cover-1.jpg"},
    @{url="https://images.unsplash.com/photo-1552566626-52f8b828add9"; name="restaurant-cover-2.jpg"},
    @{url="https://images.unsplash.com/photo-1555992336-fb0d29498b13"; name="restaurant-cover-3.jpg"},
    @{url="https://images.unsplash.com/photo-1590846406792-0adc7f938f1d"; name="restaurant-cover-4.jpg"},
    # Additional restaurant cover images
    @{url="https://images.unsplash.com/photo-1537047902294-62a40c20a6ae"; name="restaurant-cover-5.jpg"},
    @{url="https://images.unsplash.com/photo-1559339352-11d035aa65de"; name="restaurant-cover-6.jpg"},
    @{url="https://images.unsplash.com/photo-1514933651103-005eec06c04b"; name="restaurant-cover-7.jpg"},
    @{url="https://images.unsplash.com/photo-1581954756481-e5fdb0a892b2"; name="restaurant-cover-8.jpg"},
    # Restaurant logo images
    @{url="https://images.unsplash.com/photo-1594041680534-e8c8cdebd659"; name="restaurant-logo-1.jpg"},
    @{url="https://images.unsplash.com/photo-1608031003385-c3992f10dfb5"; name="restaurant-logo-2.jpg"},
    @{url="https://images.unsplash.com/photo-1566478989037-eec170784d0b"; name="restaurant-logo-3.jpg"},
    @{url="https://images.unsplash.com/photo-1555992457-b8fefdd70bef"; name="restaurant-logo-4.jpg"},
    # Additional restaurant logo images
    @{url="https://images.unsplash.com/photo-1577219491135-ce391730fb2c"; name="restaurant-logo-5.jpg"},
    @{url="https://images.unsplash.com/photo-1622419341006-bca64904e936"; name="restaurant-logo-6.jpg"},
    @{url="https://images.unsplash.com/photo-1615250963003-97d88de1a34e"; name="restaurant-logo-7.jpg"},
    @{url="https://images.unsplash.com/photo-1595708684082-a173bb3a06c5"; name="restaurant-logo-8.jpg"},
    # Food dish images
    @{url="https://images.unsplash.com/photo-1565299624946-b28f40a0ae38"; name="dish-1.jpg"},
    @{url="https://images.unsplash.com/photo-1546069901-ba9599a7e63c"; name="dish-2.jpg"},
    @{url="https://images.unsplash.com/photo-1567620905732-2d1ec7ab7445"; name="dish-3.jpg"},
    @{url="https://images.unsplash.com/photo-1565958011703-44f9829ba187"; name="dish-4.jpg"},
    @{url="https://images.unsplash.com/photo-1565299507177-b0ac66763828"; name="dish-5.jpg"},
    @{url="https://images.unsplash.com/photo-1540189549336-e6e99c3679fe"; name="dish-6.jpg"},
    @{url="https://images.unsplash.com/photo-1512621776951-a57141f2eefd"; name="dish-7.jpg"},
    @{url="https://images.unsplash.com/photo-1551782450-17144efb9c50"; name="dish-8.jpg"},
    # Additional food dish images
    @{url="https://images.unsplash.com/photo-1572802419224-296b0aeee0d9"; name="dish-9.jpg"},
    @{url="https://images.unsplash.com/photo-1586190848861-99aa4a171e90"; name="dish-10.jpg"},
    @{url="https://images.unsplash.com/photo-1559742811-822873691df8"; name="dish-11.jpg"},
    @{url="https://images.unsplash.com/photo-1563379091339-03b21ab4a4f8"; name="dish-12.jpg"},
    @{url="https://images.unsplash.com/photo-1569058242567-93de6f36f8e7"; name="dish-13.jpg"},
    @{url="https://images.unsplash.com/photo-1534080564583-6be75777b70a"; name="dish-14.jpg"},
    @{url="https://images.unsplash.com/photo-1558961363-fa8fdf82db35"; name="dish-15.jpg"},
    @{url="https://images.unsplash.com/photo-1562967916-eb82221dfb92"; name="dish-16.jpg"},
    # Category icons
    @{url="https://images.unsplash.com/photo-1571091718767-18b5b1457add"; name="category-burger.jpg"},
    @{url="https://images.unsplash.com/photo-1513104890138-7c749659a591"; name="category-pizza.jpg"},
    @{url="https://images.unsplash.com/photo-1563379926898-05f4575a45d8"; name="category-sushi.jpg"},
    @{url="https://images.unsplash.com/photo-1518133683791-0b9de5a055f0"; name="category-dessert.jpg"},
    # Additional category icons
    @{url="https://images.unsplash.com/photo-1553451133-8083c47243d6"; name="category-drinks.jpg"},
    @{url="https://images.unsplash.com/photo-1607532941433-304659e8198a"; name="category-vegetarian.jpg"},
    @{url="https://images.unsplash.com/photo-1558199141-391d935676f0"; name="category-breakfast.jpg"},
    # App UI images
    @{url="https://images.unsplash.com/photo-1577106263724-2c8e03bfe9cf"; name="empty-cart.jpg"},
    @{url="https://images.unsplash.com/photo-1607605774980-0df5eeb3d113"; name="success-order.jpg"}
)

$downloadPath = "$PSScriptRoot"

foreach ($image in $imageUrls) {
    $outputPath = "$downloadPath\$($image.name)"
    Write-Host "Downloading $($image.name)..."
    Invoke-WebRequest -Uri "$($image.url)?w=800&q=80" -OutFile $outputPath
}

Write-Host "All images downloaded successfully!"
