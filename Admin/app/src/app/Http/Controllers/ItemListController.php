<?php

namespace App\Http\Controllers;

use App\Models\Item;
use Illuminate\Http\Request;

use Illuminate\Support\Facades\Validator;

class ItemListController extends Controller
{
    public function ItemList(Request $request)
    {
        //アイテムリストのデータを全て取得
        //$items = Item::all();
        $items = Item::Paginate(5);
        return view('items.itemList', ['accounts' => $items]);

    }
}
